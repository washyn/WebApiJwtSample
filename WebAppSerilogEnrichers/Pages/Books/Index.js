$(function () {
  var l = abp.localization.getResource("BookStore");
  var createModal = new abp.ModalManager(abp.appPath + "Books/CreateModal");
  var editModal = new abp.ModalManager(abp.appPath + "Books/EditModal");

  var dataTable = $("#BooksTable").DataTable(
    abp.libs.datatables.normalizeConfiguration({
      order: [[3, "asc"]],
      serverSide: true,
      paging: true,
      searching: false,
      responsive: true,
      ajax: abp.libs.datatables.createAjax(
        washyn.bookStore.services.books.book.getList
      ),
      columnDefs: [
        {
          title: l("CreationTime"),
          data: "creationTime",
          dataFormat: "datetime",
        },
        {
          title: l("Name"),
          data: "name",
        },
        // add some way for manage enums
        {
          title: l("Type"),
          data: "type",
          render: function (data) {
            return l("Enum:BookType." + data);
            // return l("Enum:BookType." + 1-7);
          },
        },
        {
          title: l("PublishDate"),
          data: "publishDate",
          dataFormat: "date",
        },
        {
          title: l("Price"),
          data: "price",
        },
        {
          title: l("Actions"),
          // can be add another way to manage rowAction
          // customAction: {
          //   // can be add only this context for manage rowAction
          //   deleteAction: delete (editModal, true, datatable),
          //   editAction: edit(
          //     datatable,
          //     editModal,
          //     washyn.bookStore.services.books.book,
          //     true
          //   ),
          // },
          rowAction: {
            items: [
              {
                text: l("Edit"),
                visible: true,
                action: function (data) {
                  editModal.open({ id: data.record.id });
                },
              },
              {
                text: l("Delete"),
                visible: true,
                confirmMessage: function (data) {
                  return l("BookDeletionConfirmationMessage", data.record.name);
                },
                action: function (data) {
                  washyn.bookStore.services.books.book
                    .delete(data.record.id)
                    .then(function () {
                      abp.notify.success(l("DeletedSuccessfully"));
                      dataTable.ajax.reload();
                    });
                },
              },
            ],
          },
        },
      ],
    })
  );
  // can be add as default in on result code
  createModal.onResult(function () {
    abp.notify.success(l("CreatedSuccessfully"));
    dataTable.ajax.reload();
  });
  // can be add as default in on result code of edit modal
  editModal.onResult(function () {
    abp.notify.success(l("SavedSuccessfully"));
    dataTable.ajax.reload();
  });

  $("#NewBookButton").click(function (e) {
    e.preventDefault();
    createModal.open();
  });
});
