var divRes1Name = "MainContent_pExampleResultBlock1String";
var divRes2Name = "MainContent_pExampleResultBlock2String";
var divRes3Name = "MainContent_pExampleResultBlock3String";
var divRes4Name = "MainContent_pExampleResultBlock4String";
var divRes5Name = "MainContent_pExampleResultBlock5String";
var divRes6Name = "MainContent_pExampleResultBlock6String";
var divRes7Name = "MainContent_pExampleResultBlock7String";
var divRes8Name = "MainContent_pExampleResultBlock8String";

var resArr = [divRes1Name, divRes2Name, divRes3Name, divRes4Name, divRes5Name, divRes6Name, divRes7Name, divRes8Name];

$(function () {
    $('.spoiler-body').hide(300);
    $(document).on('click', '.spoiler-head', function (e) {
        e.preventDefault()
        $(this).parents('.spoiler-wrap').toggleClass("active").find('.spoiler-body').slideToggle();
    })
})

function expandFilledBlock() {
    resArr.forEach(function (element) {
        if (document.getElementById(element).hasChildNodes()) {
            console.log(element);
            var parent = document.getElementById(element).parentNode.parentNode.parentNode;
            parent.getElementsByTagName("a")[0].click();
            setTimeout(function () {
                $(parent)[0].scrollIntoView(true);
            }, 100);
            return;
        }
    });
}

setTimeout(function () {
    expandFilledBlock();
}, 500);

function showTimer() {

    // Set the date we're counting down to
    var tokenIssueDate = document.getElementById("MainContent_pExpirationTime").innerHTML;
    var countDownDate = new Date(tokenIssueDate * 1000).getTime();
    // console.log(countDownDate);

    // Update the count down every 1 second
    var x = setInterval(function () {

        // checkRefreshButton();

        // Get todays date and time
        var now = new Date().getTime();

        // Find the distance between now and the count down date
        var distance = countDownDate - now;

        // Time calculations for days, hours, minutes and seconds
        var days = Math.floor(distance / (1000 * 60 * 60 * 24));
        var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        var seconds = Math.floor((distance % (1000 * 60)) / 1000);

        // Output the result in an element with id="demo"
        document.getElementById("expires-label").innerHTML = "Expires in";
        document.getElementById("expires-in").innerHTML = minutes + "m " + seconds + "s ";

        // If the count down is over, write some text 
        if (distance < 0) {
            clearInterval(x);
            var tokenLabel = document.getElementById("MainContent_pAuthorizationToken");
            if (tokenLabel && tokenLabel.lenght > 0) {
                document.getElementById("expires-label").innerHTML = "Expired";
                document.getElementById("expires-in").innerHTML = "";
            } else {
                document.getElementById("expires-label").innerHTML = "";
                document.getElementById("expires-in").innerHTML = "";
            }

        }
    }, 1000);
}
