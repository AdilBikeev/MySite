var countAnswer=1;

$(document).ready(function () {
    $("button").click(function () { // задаем функцию при нажатиии на элемент <button>
        countAnswer++;
        $("tr[name=\"example\"]:last").after("<tr name = \"example\"><td>" + countAnswer + " Вариант ответа &nbsp;</td > <td><textarea name=\"AnswerChoice\">Вариант ответа</textarea> </td></tr>"); //  вставляем содержимое, указанное в параметре метода в конец каждого выбранного элемента <p>
        $("select:last").append("<option>" + countAnswer +" Вариант ответа</option>")
    });
});