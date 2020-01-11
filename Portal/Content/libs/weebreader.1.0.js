$.fn.api.settings.successTest = function(response) {
    return response.success;
};

function addError(element, response) {
    element.removeClass("success");
    element.addClass("error");
    addMessages(element, response);
}

function addSuccess(element, response) {
    element.removeClass("error");
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