$.fn.api.settings.successTest = (response) => response.success;

$(function () {
    $(".ui.checkbox").checkbox();
    $("select.dropdown").dropdown();

    $(".message .close").on("click", function() {
        $(this).closest(".message").transition("fade");
    });
    
    $(".form").on("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            
            return false;
        }
    });

    $("input[type=\"file\"]").on("change", function (event) {
        $("label.button").html("<i class=\"file icon\"></i>" + event.target.files[0].name.substr(0, 14) + (event.target.files[0].name.length > 15 ? "..." : ""));
    });

    $(".ui.dropdown").dropdown({
        allowAdditions: true
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

function addGenericError(element) {
    let mockResponse = {
        messages: ["Something went wrong, please try again or contact an adminsitrator."]
    };

    addError(element, mockResponse)
}

function addMessages(element, response) {
    element.empty();
    let list = element.append("<ul class=\"list\"></ul>").find("ul");
    response.messages.forEach(function (currentValue) {
        list.append("<li>" + currentValue + "</li>")
    });
}

function animateProgressBar(element, event) {
    element.progress({
        percent: event.loaded / event.total * 100
    });
}

function buildTable(element, data) {
    let body = element.children("tbody");
    body.empty();
    
    data.forEach(function (value) {
        body.append("<tr class=\"center aligned\" data-id=\"" + value.id + "\">" + "<td>" + value.name + "</td>" + "<td>" + value.status + "</td>" + "<td><div class=\"ui small buttons\"><button class=\"ui icon button primary\"><i class=\"pen icon\"></i></button>" +
            "<button class=\"ui icon button negative\"><i class=\"trash icon\"></i></button></div></td>" + "</tr>");
    })
}