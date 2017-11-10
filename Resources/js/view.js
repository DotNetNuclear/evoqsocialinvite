var dnnuclear = dnnuclear || {};
(function (dnnuclear) {
    dnnuclear.formOptions = {
        ModuleId: 0,
        ModuleName: "",
        FormPanelId: "#divFormPanel",
        AlertPanelId: "#divAlert",
        LoadingImageId: "#imgLoading",
        MaxEmailInputs: 10,
        MaxDailyInvites: 50,
        DailyInviteCount: 0
    }

    dnnuclear.InviteList = function InviteList(inviteData, context) {
        $this = this;
        $this.VM = new InviteListModel(inviteData.Invitations);
        ko.applyBindings($this.VM, context);
    };

    dnnuclear.InviteForm = function InviteForm(options) {
        /* Constructor */
        $this = this;
        $this.Options = $.extend(dnnuclear.formOptions, options);

        /* Methods */
        this.fadeInvitePanel = function (fadeOut) {
            var loadingImg = $($this.Options.LoadingImageId);
            var fadePanel = $($this.Options.FormPanelId);
            if (fadeOut) {
                $(loadingImg).show();
                $(fadePanel).css('opacity', 0.4);
            } else {
                $(loadingImg).hide();
                $(fadePanel).css('opacity', 1);
            }
        };
        this.validateAndSubmit = function (evt, ctx) {
            if (ctx) { $this = ctx; }

            // Ascend from the button that triggered this click event 
            //  until we find a container element flagged with 
            //  .validationGroup and store a reference to that element.
            var $group = $(evt.currentTarget).parents('.validationGroup');

            var isValid = true;

            // Descending from that .validationGroup element, find any input
            //  elements within it, iterate over them, and run validation on 
            //  each of them.
            $group.find(':input').each(function (i, item) {
                if (!$(item).valid())
                    isValid = false;
            });

            // If any fields failed validation, prevent the button's click 
            //  event from triggering form submission.
            if (!isValid) {
                evt.preventDefault();
            } else {
                $this.fadeInvitePanel(true);
                var toList = new Array();
                var emList = ko.utils.unwrapObservable($this.VM.emailModel);
                for (var i = 0; i < emList.length; i++) { toList.push(emList[i].trimmed()); }
                var slInviteSf = $.ServicesFramework($this.Options.ModuleId);
                var serviceRoot = slInviteSf.getServiceRoot($this.Options.ModuleName);

                $.ajax({
                    type: "POST",
                    url: serviceRoot + "invite/send",
                    beforeSend: slInviteSf.setModuleHeaders,
                    data: { "toList": toList, "note": $this.VM.note() },
                    dataType: "json"
                }).done(function (response) {
                    var msgt = 'dnnFormSuccess';
                    //if (__InviteListing) {
                    //    __InviteListing.VM.invitesModel(response.Invitations);
                    //}
                    $this.VM.emailModel.removeAll();
                    $this.VM.emailModel([{ text: "", trimmed: ko.observable('').trimmed() }]);
                    if (response.Warnings > 0) { var msgt = 'dnnFormWarning'; }
                    if (response.Errors > 0) { var msgt = 'dnnFormError'; }
                    $this.fadeInvitePanel(false);
                    $this.alertText(response.Messages.join('<br>'), msgt);
                }).fail(function (xhr, result, error) {
                    //alert("status: " + xhr.status + "\n" + error);
                    $this.fadeInvitePanel(false);
                });
            }
        };
        $this.alertText = function (msg, typeClass, frmCtrl) {
            if (frmCtrl) { $this = frmCtrl; }
            var alertPanel = $($this.Options.AlertPanelId);
            $(alertPanel).find('div').html(msg);
            $(alertPanel).attr('class', 'dnnFormMessage').addClass(typeClass).hide().show();
            setTimeout(function () { $($this.Options.AlertPanelId).fadeOut("slow") }, 5000);
        };
        $this.initValidation = function () {
            // Initialize validation on the entire ASP.NET form.
            $("form:first").validate({
                onsubmit: false,
                errorPlacement: function (label, element) {
                    label.addClass('formErrorContent');
                    label.insertAfter(element);
                    $(label).css('left', $(element).width() - ($(label).width() / 2));
                },
                wrapper: 'div'
            });
        }

        /* Initialize */
        $this.fadeInvitePanel(false);
        $this.initValidation();

        $this.VM = new InviteModel($this);
        ko.applyBindings($this.VM, $(options.FormPanelId).get(0));
        $('.validationGroup :text').keydown(function (evt) {
            // Only execute validation if the key pressed was enter.
            if (evt.keyCode === 13) {
                $this.validateAndSubmit(evt);
            }
        });
    };

    function InviteListModel(data) {
        var $self = this;
        $self.displayInviteList = ko.observable(true);
        $self.invitesModel = ko.observable(data);
    }

    function InviteModel($formController) {
        var $self = this;
        $self.emailCount = 1;
        $self.maxEamilInputs = $formController.Options.MaxEmailInputs;
        $self.enableForm = ko.observable(($formController.Options.DailyInviteCount < $formController.Options.MaxDailyInvites));
        $self.note = ko.observable($formController.Options.DefaultMessage);
        var initEmail = new EmailModel();
        $self.emailModel = ko.observableArray([initEmail]);
        $self.addEmail = function (data, event) {
            if ($self.emailCount < $self.maxEamilInputs) {
                var txtEmail = $(event.target).parent().parent().find('input:first');
                $self.emailModel.push({ text: txtEmail.val(), trimmed: ko.observable($.trim(txtEmail.val())).trimmed() });
                txtEmail.val('');
                $self.emailCount++;
            } else {
                $formController.alertText('You have exceeded the maximum number of email boxes.', 'dnnFormWarning', $formController);
            }
        };
        $self.sendInvite = function (data, event) {
            if ($formController.validateAndSubmit) {
                $formController.validateAndSubmit(event, $formController);
            }
        };
        $self.removeEmail = function () {
            $self.emailModel.remove(this);
            $self.emailCount--;
        };
        $self.initValidationRules = function (elem) { initEmailValidator(elem); };
        $self.removeInvite = function (elem) {
            if (elem.nodeType === 1) {
                $(elem).fadeOut(function () { $(elem).remove(); });
            }
        };
    };
    
    function EmailModel() {
        var $self = this;
        $self.text = ko.observable("");
        $self.trimmed = ko.observable("").trimmed();
    }

    ko.subscribable.fn.trimmed = function () {
        return ko.computed({
            read: function () {
                return $.trim(this());
            },
            write: function (value) {
                this($.trim(value));
                this.valueHasMutated();
            },
            owner: this
        });
    };

    /* UTILITIES */
    function initEmailValidator(elem) {
        var emailValRules = {
            required: true, email: true,
            messages: {
                required: 'Oops, you forgot to enter an email!',
                email: 'Oops, the email you entered doesn\'t look right!'
            }
        };
        if ($(elem) !== undefined) {
            $(elem).find("input").rules("add", emailValRules);
        } else {
            $('input[id^="email_"]').rules("add", emailValRules);
        }
    }
}(dnnuclear));

function fixJsonDate(dateText) {
    var d = new Date(parseInt(dateText.replace("/Date(", "").replace(")/", ""), 10));
    return (d.getMonth() + 1) + '/' + d.getDate() + '/' + d.getFullYear();
}