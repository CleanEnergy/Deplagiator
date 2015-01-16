/// <reference path="../jquery-2.1.1.js" />


/*Globals*/
var paginationOn = false;
var sortingOn = false;
var filteringOn = false;

/*Sorting*/
function parseDate(value, format) {
    var obj = { day: 0, month: 0, year: 0 };

    switch (format) {
        case 'm/d/yyyy':
            {
                var values = value.split('/');
                obj.year = parseInt(values[2]);
                obj.month = parseInt(values[0]);
                obj.day = parseInt(values[1]);
                return obj;
            } break;
        case 'd/m/yyyy':
            { } break;
        case 'mm/dd/yyyy':
            { } break;
        case 'dd/mm/yyyy':
            { } break;
        case 'm.d.yyyy':
            { } break;
        case 'd.m.yyyy':
            { } break;
        case 'mm.dd.yyyy':
            { } break;
        case 'dd.mm.yyyy':
            { } break;
        default:

    }
}
function compareDate(d1, d2) {
    if (d1.year > d2.year) {
        return -1;
    }
    else if (d2.year > d1.year) {
        return 1;
    }
    else if (d1.year == d2.year) {
        if (d1.month > d2.month) {
            return -1;
        }
        else if (d2.month > d1.month) {
            return 1;
        }
        else if (d2.month == d1.month) {
            if (d1.day > d2.day) {
                return -1;
            }
            else if (d2.day > d1.day) {
                return 1;
            }
            else if (d2.day == d1.day) {
                return 0;
            }
        }
    }
}
function qsSplitDate(array, bottom, top, isAscending) {
    var left = bottom, right = top;
    var comp = array[bottom].value;

    while (left < right) {
        if (isAscending) {
            while (compareDate(array[left].value, comp) == 1 && left <= top) {
                left++;
            }
            while (compareDate(array[right].value, comp) == -1 && right >= bottom) {
                right--;
            }
        }
        else {
            while (compareDate(array[left].value, comp) == -1 && left <= top) {
                left++;
            }
            while (compareDate(array[right].value, comp) == 1 && right >= bottom) {
                right--;
            }
        }
        if (compareDate(array[left].value, array[right].value) == 0)
            left++;
        else if (left < right) {
            var tmp = array[left];
            array[left] = array[right];
            array[right] = tmp;
        }
    }
    return right;
}
function quickSortDate(array, start, end, isAscending) {
    if (start < end) {
        var j = qsSplitDate(array, start, end, isAscending);
        quickSortDate(array, start, j - 1, isAscending);
        quickSortDate(array, j + 1, end, isAscending);
    }
}
function qsSplit(array, bottom, top, isAscending) {
    var left = bottom, right = top;
    var comp = array[bottom].value;

    while (left < right) {
        if (isAscending) {
            while (array[left].value < comp && left <= top) {
                left++;
            }
            while (array[right].value > comp && right >= bottom) {
                right--;
            }
        }
        else {
            while (array[left].value > comp && left <= top) {
                left++;
            }
            while (array[right].value < comp && right >= bottom) {
                right--;
            }
        }
        if (array[left].value === array[right].value)
            left++;
        else if (left < right) {
            var tmp = array[left];
            array[left] = array[right];
            array[right] = tmp;
        }
    }
    return right;
}
function quickSort(array, start, end, isAscending) {
    if (start < end) {
        var j = qsSplit(array, start, end, isAscending);
        quickSort(array, start, j - 1, isAscending);
        quickSort(array, j + 1, end, isAscending);
    }
}
var tableConfig = [];
var rowCollection = [];
function sort(ascending, columnIndex) {
    var sortObjectArray = [];

    //Determine the type of data in the column that is to be ordered
    var type = '';
    var dateFormat = '';
    for (var i = 0; i < tableConfig.length; i++) {
        if (tableConfig[i].colIndex === columnIndex) {
            type = tableConfig[i].type;
            if (type === 'date')
                dateFormat = tableConfig[i].format;
            break;
        }
    }
    //Depending on the type, create an array of sort objects and call the respective QuickSort implementation

    var valToCompare = null;
    for (var i = 0; i < rowCollection.length; i++) {
        valToCompare = type === 'string' ? rowCollection[i].children[columnIndex].innerHTML
            : type === 'integer' ? parseInt(rowCollection[i].children[columnIndex].innerHTML)
            : type === 'float' ? parseFloat(rowCollection[i].children[columnIndex].innerHTML)
            : type === 'date' ? parseDate(rowCollection[i].children[columnIndex].innerHTML, dateFormat)
            : 'error';
        sortObjectArray.push({ contentRow: rowCollection[i], value: valToCompare });
    }

    if (type === 'date')
        quickSortDate(sortObjectArray, 0, sortObjectArray.length - 1, ascending);
    else
        quickSort(sortObjectArray, 0, sortObjectArray.length - 1, ascending);

    //Once sorted if the column did not contain dates, apply the newly ordered html content to the table
    var table = document.getElementsByClassName('total-table')[0];
    //Find the tables <tbody> and reset it's content
    var tbody = null;
    for (var i = 0; i < table.childNodes.length; i++) {
        if (table.childNodes[i].tagName == 'TBODY') {
            tbody = table.childNodes[i];
            break;
        }
    }

    //Enter the newly ordered rows into <tbody>
    if (!sortingOn) {
        for (var i = 0; i < sortObjectArray.length; i++) {
            sortObjectArray[i].contentRow.style.display = 'table-row';
            tbody.appendChild(sortObjectArray[i].contentRow);
        }
    } else {
        for (var i = 0; i < sortObjectArray.length; i++) {
            if (!sortObjectArray[i].contentRow.classList.contains('filteredOff')) {
                sortObjectArray[i].contentRow.style.display = 'table-row';
                tbody.appendChild(sortObjectArray[i].contentRow);
            }
        }
    }
    if (paginationOn)
        showPage(activePageNumber);

}
function initializeSorting(cols) {
    sortingOn = true;
    //Store the table config array
    tableConfig = cols;

    //Find the table to be sorted
    var table = document.getElementsByClassName('total-table')[0];

    //Find the tables <thead>
    var thead = null;
    for (var i = 0; i < table.childNodes.length; i++) {
        if (table.childNodes[i].tagName == 'THEAD') {
            thead = table.childNodes[i];
            break;
        }
    }

    //Find the <tr> contained in <thead>
    var headRow = null;
    for (var i = 0; i < thead.childNodes.length; i++) {
        if (thead.childNodes[i].tagName == 'TR') {
            if(thead.childNodes[i].className === 'header-row')
                headRow = thead.childNodes[i];
        }
    }

    ///Find all the <th> elements in the head row
    var headers = [];
    for (var i = 0; i < headRow.childNodes.length; i++) {
        if (headRow.childNodes[i].tagName == 'TH')
            headers.push(headRow.childNodes[i]);
    }

    //Find each header that has to be sorted and append sorting links
    for (var i = 0; i < cols.length; i++) {
        var html = headers[cols[i].colIndex].innerHTML;
        var links = '   <a href="#" onclick="sort(true,' + cols[i].colIndex + ')">Asc</a>    <a href="#" onclick="sort(false,' + cols[i].colIndex + ')">Desc</a>';
        html += links;
        headers[cols[i].colIndex].innerHTML = html;
    }

    //Find the tables <tbody>
    var tbody = null;
    for (var i = 0; i < table.childNodes.length; i++) {
        if (table.childNodes[i].tagName == 'TBODY') {
            tbody = table.childNodes[i];
            break;
        }
    }

    //Find all the rows in <tbody>
    var rows = [];
    var tbodyChildren = tbody.childNodes;
    for (var i = 0; i < tbodyChildren.length; i++) {
        if (tbodyChildren[i].tagName == 'TR') {
            //Store all the original rows
            if (!tbodyChildren[i].classList.contains('total-ignore'))
                rowCollection.push(tbody.childNodes[i]);
        }
    }
}

/*Pagination*/
var totalRows = 0;
var pageSize = 0;
var activePageNumber = 0;
var totalPages = 0;
function initializePaging(pSize) {

    paginationOn = true;
    pageSize = pSize;
    countTotalRows();
    paginate();
}
/*Sets new values to the global variables, redisplays the navigation links and <select> element
  and shows the first page*/
function paginate() {
    var navElement = document.getElementById('pagination-nav');
    navElement.innerHTML = '';

    showNavigationSelect();
    showPagesNavigation();

    countTotalPages();
    showPage(1);
}
function showNavigationSelect() {
    /*Find the navigation container*/
    var navElement = document.getElementById('pagination-nav');

    var rowSizeWrap = document.createElement('div');
    rowSizeWrap.className = 'pg-row-size-wrap';

    var decor = document.createElement('span');
    decor.innerHTML = "Rows per page: ";

    /*Reset the page size select*/
    var pageSizeSelect = document.createElement('select');
    var html = '';
    pageSizeSelect.id = 'pagination-page-size-select';
    html += '<option value="10">10</option>';
    html += '<option value="25">25</option>';
    html += '<option value="50">50</option>';
    html += '<option value="100">100</option>';
    html += '<option value="500">500</option>';
    pageSizeSelect.innerHTML = html;
    /*When a value in the page size changes, pagination() is called*/
    pageSizeSelect.onchange = function () {
        var selectElement = document.getElementById('pagination-page-size-select');
        pageSize = selectElement.options[selectElement.selectedIndex].value;
        paginate();
    };

    rowSizeWrap.appendChild(decor);
    rowSizeWrap.appendChild(pageSizeSelect);

    navElement.appendChild(rowSizeWrap);
    /*Check the page-size <select> and set the selected attribute to the currently selected page size*/
    for (var i = 0; i < pageSizeSelect.options.length; i++) {
        if (pageSizeSelect.options[i].value == pageSize) {
            var attr = document.createAttribute('selected');
            attr.value = 'selected';
            pageSizeSelect.options[i].attributes.setNamedItem(attr);
            break;
        }
    }
}
function showPage(pageNumber) {
    countTotalRows();
    countTotalPages();
    if (pageNumber <= totalPages && pageNumber > 0 || pageNumber === 1) {
        /*Find the table*/
        var table = document.getElementsByClassName('total-table')[0];
        /*Find the <tbody>*/
        var tChildren = table.children;
        var tBody = null;
        for (var i = 0; i < tChildren.length; i++) {
            if (tChildren[i].tagName === 'TBODY') {
                tBody = tChildren[i];
            }
        }
        /*Go through all the elements and show the appropriate rows*/
        var tbChildren = tBody.children;
        var shownRows = 0;
        var rowCounter = 0;
        var canShow = false;
        var firstRowIndex = pageNumber * pageSize - pageSize;
        if (!filteringOn) {
            for (var i = 0; i < tbChildren.length; i++) {
                if (tbChildren[i].tagName === 'TR') {
                    if (rowCounter === firstRowIndex) {
                        canShow = true;
                    }
                    if (canShow && shownRows < pageSize) {
                        tbChildren[i].style.display = 'table-row';
                        shownRows++;
                    }
                    else {
                        tbChildren[i].style.display = 'none';
                    }
                    rowCounter++;
                }
            }
        } else {
            for (var i = 0; i < tbChildren.length; i++) {
                if (tbChildren[i].tagName === 'TR') {
                    if (!tbChildren[i].classList.contains('filteredOff')) {
                        if (rowCounter === firstRowIndex) {
                            canShow = true;
                        }
                        if (canShow && shownRows < pageSize) {
                            tbChildren[i].style.display = 'table-row';
                            shownRows++;
                        }
                        else {
                            tbChildren[i].style.display = 'none';
                        }
                        rowCounter++;
                    }
                }
            }
        }
        
        activePageNumber = pageNumber;
        var statSpan = document.getElementById('pagination-stat-span');
        statSpan.innerText = 'Showing ' + activePageNumber + ' / ' + totalPages + ' pages';
    }
}
function showPagesNavigation() {

    var navElement = document.getElementById('pagination-nav');
    var navControls = document.createElement('div');
    navControls.classList.add('pagination-controls');


    var prevBtn = document.createElement('input');
    prevBtn.type = 'button';
    prevBtn.value = '<';
    prevBtn.className = 'btn btn-default btn-pg-left';
    prevBtn.onclick = function () { showPage(activePageNumber - 1); }

    var nextBtn = document.createElement('input');
    nextBtn.type = 'button';
    nextBtn.value = '>';
    nextBtn.className = 'btn btn-default btn-pg-right';
    nextBtn.onclick = function () { showPage(activePageNumber + 1); }

    var pageNumTB = document.createElement('input');
    pageNumTB.type = 'text';
    pageNumTB.classList.add('form-control');
    pageNumTB.classList.add('pagination-nav-tb');

    var showPageBtn = document.createElement('input');
    showPageBtn.type = 'button';
    showPageBtn.value = 'Go';
    showPageBtn.className = 'btn btn-default';
    showPageBtn.onclick = function () {
        var val = pageNumTB.value;
        if (parseInt(val) != 'NaN') {
            showPage(parseInt(val));
        }
    }

    var statSpan = document.createElement('label');
    statSpan.className = 'label-pg-stat';
    statSpan.id = 'pagination-stat-span';
    statSpan.innerText = 'Showing ' + activePageNumber + ' / ' + totalPages + ' pages';

    navControls.appendChild(prevBtn);
    navControls.appendChild(pageNumTB);
    navControls.appendChild(showPageBtn);
    navControls.appendChild(nextBtn);

    navElement.appendChild(navControls);

    navElement.appendChild(statSpan);
}
function countTotalRows() {
    /*Find the table*/
    var table = document.getElementsByClassName('total-table')[0];
    /*Find the <tbody>*/
    var tChildren = table.children;
    var tBody = null;
    for (var i = 0; i < tChildren.length; i++) {
        if (tChildren[i].tagName === 'TBODY') {
            tBody = tChildren[i];
        }
    }
    /*Go through all the elements and count the appropriate rows*/
    var tbChildren = tBody.children;
    var rowCounter = 0;
    for (var i = 0; i < tbChildren.length; i++) {
        if (tbChildren[i].tagName === 'TR') {
            if (!tbChildren[i].classList.contains('filteredOff')) {
                rowCounter++;
            }
        }
    }
    totalRows = rowCounter;
}
function countTotalPages() {
    totalPages = Math.ceil(totalRows / pageSize);
}

/*Searching*/
var filters = [];
var tableRows = [];
var filterCols = [];

function initializeFiltering(allCols, filCols) {
    filteringOn = true;

    //Find the table to be sorted
    var table = document.getElementsByClassName('total-table')[0];

    //Find the tables <thead>
    var thead = null;
    var tableChildren = table.childNodes;
    for (var i = 0; i < tableChildren.length; i++) {
        if (tableChildren[i].tagName == 'THEAD') {
            thead = tableChildren[i];
            break;
        }
    }

    //Find the <tr class="filters-row">
    var filtersRow = null;
    var theadChildren = thead.childNodes;
    for (var i = 0; i < theadChildren.length; i++) {
        if (theadChildren[i].tagName == 'TR') {
            if (theadChildren[i].className === 'filters-row') {
                filtersRow = theadChildren[i];
                break;
            }
        }
    }

    /*Create the <td>'s and input elements where appropriate and append their classess*/
    for (var i = 0; i < allCols; i++) {
        var cell = document.createElement('td');
        for (var j = 0; j < filCols.length; j++) {
            if(i === filCols[j].index)
            {
                var inputEl = document.createElement('input');
                for (var k = 0; k < filCols[j].styleClassNames.length; k++) {
                    inputEl.classList.add(filCols[j].styleClassNames[k]);
                }
                inputEl.onkeyup = function () {
                    filter();
                }
                for (var k = 0; k < filCols[j].cellClassNames.length; k++) {
                    cell.classList.add(filCols[j].cellClassNames[k]);
                }
                cell.appendChild(inputEl);
                filters.push(inputEl);
            }
        }
        filtersRow.appendChild(cell);
    }

    filterCols = filCols;
}
function filter() {

    /*Go through the entire table and remember each row if tableRows is empty*/
    if (tableRows.length === 0) {
        var table = document.getElementsByClassName('total-table')[0];
        var tableChildren = table.children;
        var tbody = null;
        for (var i = 0; i < tableChildren.length; i++) {
            if (tableChildren[i].tagName === 'TBODY') {
                tbody = tableChildren[i];
                break;
            }
        }
        var tbodyChildren = tbody.children;

        for (var i = 0; i < tbodyChildren.length; i++) {
            if (tbodyChildren[i].tagName === 'TR') {
                tableRows.push(tbodyChildren[i]);
            }
        }
    }
    /*Go through each row, show it, then hide it depending on the filters*/
    for (var i = 0; i < tableRows.length; i++) {
        var allOk = true;
        tableRows[i].style.display = 'table-row';
        tableRows[i].classList.remove('filteredOff');
        /*Find all the cells in the row*/
        var cells = [];
        var rowChildren = tableRows[i].childNodes;
        for (var j = 0; j < rowChildren.length; j++) {
            if(rowChildren[j].tagName === 'TD')
            {
                cells.push(rowChildren[j]);
            }
        }
        /*Go through each cell and if it matches a filter index, filter it*/
        var filterIndexer = 0;
        for (var j = 0; j < cells.length; j++) {
            var filterOn = false;
            for (var k = 0; k < filterCols.length; k++) {
                if(filterCols[k].index == j)
                {
                    filterOn = true;
                }
            }
            if (filterOn) {
                var content = cells[j].textContent.replace(/\s+/g, ' ').toLowerCase();
                var filter = filters[filterIndexer];
                filterIndexer++;
                if (filter != null) {
                    if (filter.value != '') {
                        var match = ~content.indexOf(filter.value.toLowerCase());
                        if (!match) {
                            allOk = false;
                            break;
                        }
                    }
                }
            }
        }
        if (!allOk) {
            tableRows[i].classList.add('filteredOff');
            tableRows[i].style.display = 'none';
        }
    }

    if (paginationOn)
        paginate();
}

/*Deleting*/
var deleteList = [];

var confirmMessage = '';
var noneSelectedMessage = '';
var ajaxUrl = '';
var errorPage = '';

function initializeDeletion(confirmationMsg, nothingSelectedMsg, ajaxCallUrl, errorUrl) {
    confirmMessage = confirmationMsg;
    noneSelectedMessage = nothingSelectedMsg;
    ajaxUrl = ajaxCallUrl;
    errorPage = errorUrl;
}

function markForDeletion(id, element) {
    if ($(element).is(':checked')) {
        deleteList.push(id);
    }
    else {
        for (var i = 0; i < deleteList.length; i++) {
            if (deleteList[i] == id)
                deleteList.splice(i, 1);
        }
    }
}

// Calls an action with POST. Sends a parameter 'deleteList', which is a list of selected ID's.
// If the call is sucesfull, reloads the page.
// If the call fails and the exception is handled on the server, an 'errorPage' parameter is expected, to which the page redirects.
// If an unhandled exception ocurs, the page redirects to the URL defined in the constructor.
function deleteSelected() {

    if (deleteList.length != 0) {

        if (confirm(confirmMessage)) {
            $.ajax({
                url: ajaxUrl,
                method: 'post',
                data: { deleteList: JSON.stringify(deleteList) },
                success: function (res) {
                    if (res.error == null)
                        location.reload(true);
                    else
                        window.location.href = res.errorPage;
                },
                error: function (res) {
                    window.location.href = errorPage;
                }
            });
        }
    } else {
        alert(noneSelectedMessage);
    }
}

/*Initialization examples*/

// Filtering
/*

initializeFiltering(7, [
    { index: 1, styleClassNames: ['table-search-md'], cellClassNames: ['table-search-align-left'] },
    { index: 4, styleClassNames: ['table-search-md'], cellClassNames: ['table-search-align-left'] },
    { index: 5, styleClassNames: ['table-search-md'], cellClassNames: ['table-search-align-left'] }
]);

*/

// Deletion
/*

initializeDeletion(
    "@lRes.ConfirmDeleteLanguages",
    "@lRes.NoLanguageSelected",
    '/Localization/DeleteLanguages',
    "@System.Configuration.ConfigurationManager.AppSettings["errorPageUrl"]"
);

*/

// 