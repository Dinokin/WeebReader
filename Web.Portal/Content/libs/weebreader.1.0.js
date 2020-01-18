$.fn.api.settings.successTest = function(response) {
    return response.success;
};

$(function () {
    $(".form").on("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            
            return false;
        }
    });

    $(".message .close").on("click", function() {
            $(this).closest(".message").transition("fade");
        });
});

function addError(element, response) {
    element.removeClass("success hidden");
    element.addClass("error");
    addMessages(element, response);
}

function addSuccess(element, response) {
    element.removeClass("error hidden");
    element.addClass("success");
    addMessages(element, response);
}

function addMessages(element, response) {
    element.empty();
    let list = element.append("<ul class=\"list\"></ul>").find("ul");
    response.messages.forEach(function (currentValue) {
        list.append("<li>" + currentValue + "</li>")
    });
}