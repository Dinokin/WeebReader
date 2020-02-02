$.fn.api.settings.successTest = (response) => response.success;

$(function () {
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

function animateProgressBar(element, event) {
    element.progress({
        percent: event.loaded / event.total * 100
    });
}

function buildUserTable(table, data) {
    let body = table.children("tbody");
    body.empty();
    
    data.forEach((value) => {
        body.append("<tr data-id=\""+ value.id + "\">" 
            + "<td>" + value.userName + "</td>"
            + "<td>" + value.email + "</td>"
            + "<td>" + value.role + "</td>"
            + "<td>" + "<button class=\"ui icon primary mini button\"><i class=\"pen icon\"></i></button>" + "</td>"
            + "</tr>");
    });
}