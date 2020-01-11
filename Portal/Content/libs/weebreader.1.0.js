$.fn.api.settings.successTest = function(response) {
    return response.success;
};

function addError(element, response){
    element.empty();
    let list = element.append("<ul class=\"list\"></ul>").find("ul");
    
    response.problems.forEach(function (currentValue) {
        list.append("<li>" + currentValue + "</li>")
    });
}