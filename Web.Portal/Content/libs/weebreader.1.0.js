$.fn.api.settings.successTest = (response) => response.success;

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

    $(".ui.checkbox").checkbox();
    $("select.dropdown").dropdown();

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

function resetFileInputButton(element) {
    element.html("<i class=\"file icon\"></i>Open File");
}