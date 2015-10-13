
    $("[src*=Note1]").live("click", function () {
        $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
        $(this).attr("src", "Images/minus3.png");
    });
$("[src*=minus]").live("click", function () {
    $(this).attr("src", "Images/Note1.png");
    $(this).closest("tr").next().remove();
});
    //Validate Login - btn Search
    function valLogin() {
        var loginId = document.getElementById('loginId').textContent;
        var loginId = document.getElementById('loginId').textContent;

        if (loginId == '') {
            alert('Please login using your Gmail Account');
            return false;
        }
        else
            document.getElementById('hvfEmail').value = loginId;
        if (document.getElementById('txtSearch').value == '') {
            alert('I Guess you missed to type in your search content !!!');
            return false;
        }
        return true;
    }

//Validate Textbox for input
function validate() {
    var loginId = document.getElementById('loginId').textContent;

    if (loginId == '') {
        alert('Please login using your Gmail Account');
        return false;
    }
    else
        document.getElementById('hvfEmail').value = loginId;
}
