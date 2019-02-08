 var $table = $('#fresh-table'),
        full_screen = false;

        $().ready(function () {
        $table.bootstrapTable({
            toolbar: ".toolbar",

            search: true,
            pagination: true,
            striped: true,
            pageSize: 8,
            pageList: [8, 10, 25, 50, 100],

            formatShowingRows: function (pageFrom, pageTo, totalRows) {
                //do nothing here, we don't want to show the text "showing x of y from..."
            },
            formatRecordsPerPage: function (pageNumber) {
                return pageNumber + " rows visible";
            },
            icons: {
                toggle: 'fa fa-th-list',
                columns: 'fa fa-columns',
                detailOpen: 'fa fa-plus-circle',
                detailClose: 'fa fa-minus-circle'
            }
        });



    $(window).resize(function () {
        $table.bootstrapTable('resetView');
    });
});