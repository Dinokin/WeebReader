$(() => {
    let changePasswordForm = $("#change-password-form");
    let changeEmailForm = $("#change-email-form");
    let changePasswordMessage = changePasswordForm.children(".message");
    let changeEmailMessage = changeEmailForm.children(".message");

    $("#your-profile").addClass("active");

    $(".form").on("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            return false;
        }
    });

    changePasswordForm.form({
        on: "blur",
        fields: {
            password: {
                identifier: "password",
                rules: [
                    {
                        type: "empty",
                        prompt: passwordRequiredLabel
                    },
                    {
                        type: "minLength[8]",
                        prompt: minimumPasswordLengthLabel
                    }
                ]
            },
            newPassword: {
                identifier: "newPassword",
                rules: [
                    {
                        type: "empty",
                        prompt: newPasswordRequiredLabel
                    },
                    {
                        type: "minLength[8]",
                        prompt: minimumPasswordLengthLabel
                    }
                ]
            },
            repeatNewPassword: {
                identifier: "repeatNewPassword",
                rules: [
                    {
                        type: "match[newPassword]",
                        prompt: passwordsMustBeEqualLabel
                    }
                ]
            }
        }
    }).api({
        url: changePasswordRoute,
        serializeForm: true,
        method: "PATCH",
        onSuccess: response => {
            addSuccess(changePasswordMessage, response);
            changePasswordForm.form("reset");
        },
        onFailure: response => Array.isArray(response.messages) ? addError(changePasswordMessage, response) : addError(changePasswordMessage, buildFakeResponse([requestFailedLabel])),
        onError: () => addError(changePasswordMessage, buildFakeResponse([requestFailedLabel]))
    });

    changeEmailForm.form({
        on: "blur",
        fields: {
            email: {
                identifier: "email",
                rules: [
                    {
                        type: "empty",
                        prompt: emailRequiredLabel
                    },
                    {
                        type: "email",
                        prompt: validEmailRequiredLabel
                    }
                ]
            },
            repeatNewPassword: {
                identifier: "repeatEmail",
                rules: [
                    {
                        type: "match[email]",
                        prompt: emailsMustBeEqualLabel
                    }
                ]
            }
        }
    }).api({
        url: changeEmailRoute,
        serializeForm: true,
        method: "PATCH",
        onSuccess: response => {
            addSuccess(changeEmailMessage, response);
            changeEmailForm.form("reset");
        },
        onFailure: response => Array.isArray(response.messages) ? addError(changeEmailMessage, response) : addError(changeEmailMessage, buildFakeResponse([requestFailedLabel])),
        onError: () => addError(changeEmailMessage, buildFakeResponse([requestFailedLabel]))
    });
});