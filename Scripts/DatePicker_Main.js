
setpickers("from", "to");
setpickers("from_", "to_");
function setpickers(first, second) {
    $(function () {
        $("#" + first + "").datepicker({
            monthNames: ['Січень', 'Лютий', 'Березень', 'Квітень', 'Травень', 'Червень', 'Липень', 'Серпень', 'Вересень', 'Жовтень', 'Листопад', 'Грудень'],
            monthNamesShort: ['Січ', 'Лют', 'Бер', 'Кві', 'Тра', 'Чер', 'Лип', 'Сер', 'Вер', 'Жов', 'Лис', 'Гру'],
            dayNames: ['Неділя', 'Понеділок', 'Вівторок', 'Середа', 'Четвер', 'П\'ятниця', 'Субота'],
            dayNamesShort: ['Нед', 'Пон', 'Вів', 'Сер', 'Чет', 'П\'я', 'Суб'],
            dayNamesMin: ['Нд', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
            defaultDate: "+1w",
            changeMonth: true,
            numberOfMonths: 2,
            onClose: function (selectedDate) {
                $("#" + first + "").datepicker("option", "minDate", selectedDate);
            }
        });
        $("#" + second + "").datepicker({
            monthNames: ['Січень', 'Лютий', 'Березень', 'Квітень', 'Травень', 'Червень', 'Липень', 'Серпень', 'Вересень', 'Жовтень', 'Листопад', 'Грудень'],
            monthNamesShort: ['Січ', 'Лют', 'Бер', 'Кві', 'Тра', 'Чер', 'Лип', 'Сер', 'Вер', 'Жов', 'Лис', 'Гру'],
            dayNames: ['Неділя', 'Понеділок', 'Вівторок', 'Середа', 'Четвер', 'П\'ятниця', 'Субота'],
            dayNamesShort: ['Нед', 'Пон', 'Вів', 'Сер', 'Чет', 'П\'я', 'Суб'],
            dayNamesMin: ['Нд', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
            defaultDate: "+1w",
            changeMonth: true,
            numberOfMonths: 2,
            onClose: function (selectedDate) {
                $("#" + second + "").datepicker("option", "maxDate", selectedDate);
            }
        });

    });
}