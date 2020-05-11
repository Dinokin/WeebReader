$.fn.api.settings.successTest = (response) => response.success;

$(() => {
    $(".form").on("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            
            return false;
        }
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
    response.messages.forEach(currentValue => {
        list.append("<li>" + currentValue + "</li>")
    });
}

function animateProgressBar(element, event) {
    element.progress({
        percent: Math.round(event.loaded / event.total * 100)
    });
}

function getCookie(name) {
    let cookie = document.cookie.match(`(^|;) ?${name}=([^;]*)(;|$)`);

    return cookie ? cookie[2] : null;
}

function setCookie(name, value, days) {
    let date = new Date;
    date.setTime(date.getTime() + 24*60*60*1000*days);
    document.cookie = name + "=" + value + ";path=/;expires=" + date.toDateString();
}

function scrollToTop(element) {
    $(window).scrollTop($(element).offset().top);
}

function buildFakeResponse(messages) {
    return {
        success: false,
        messages: messages
    };
}

function getDate(date) {
    if (!date)
        return "";

    let year = date.getFullYear();
    let month = date.getMonth() + 1;
    let day = date.getDate();

    return `${year}-${month <= 9 ? `0${month}` : month}-${day <= 9 ? `0${day}` : day}`;
}

function getTime(date) {
    if (!date)
        return "";

    let hour = date.getHours();
    let minutes = date.getMinutes();

    return `${hour <= 9 ? `0${hour}` : hour}:${minutes <= 9 ? `0${minutes}` : minutes}`;
}