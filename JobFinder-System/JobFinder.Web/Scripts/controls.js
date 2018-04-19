var Controls = function ($) {

    function multipleListControl(selectContainerId, modelName, resultContainerId, maxSelections) {
        var MAX_SELECTIONS = maxSelections;
        var selectedItems = 0;
        var resultContainer = $("#" + resultContainerId);

        $("#" + selectContainerId).change(function (e) {
            if (MAX_SELECTIONS && selectedItems === MAX_SELECTIONS) {
                return;
            }
            else {
                var optionSelected = $("option:selected", this);

                if (optionSelected.hasClass("selected")) {
                    return;
                }
                optionSelected.css('color', '#aaa');
                optionSelected.addClass("selected");
                var toAdd = $('<option>').text(optionSelected.text());
                toAdd.attr("value", this.value);

                var inp = $('<input>').attr({ 'name': modelName, 'value': this.value });
                toAdd.append(inp);

                resultContainer.append(toAdd);
                selectedItems++;
            }
        });

        resultContainer.change(function (e) {
            var optionSelected = $("option:selected", this)[0];
            this.removeChild(optionSelected);
            var selector = '#' + selectContainerId + ' option[value=' + optionSelected.value + ']';
            $(selector).removeClass("selected").css('color', '#000');
            selectedItems--;
        });
    }

    function changeButtonTextControl(elementId, firstText, secondText) {
        $('#' + elementId).click(clickHandler);

        function clickHandler() {
            var element = $('#' + elementId);
            var text = element.text();
            if (text === firstText) {
                element.text(secondText);                
            }
            else {
                element.text(firstText);
            }
        };
    };

    return {
        multipleListControl: multipleListControl,
        changeButtonTextControl : changeButtonTextControl
    }

}(jQuery)