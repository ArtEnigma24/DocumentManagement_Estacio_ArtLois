$(document).ready(function () {
   var API = "https://localhost:7241/api/documents";
    var MAX_SIZE = 10 * 1024 * 1024;

    function showMessage(text, type) {
        $("#message").attr("class", type).text(text);
    }

    function formatSize(bytes) {
        if (bytes < 1024) return bytes + " B";
        if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + " KB";
        return (bytes / (1024 * 1024)).toFixed(2) + " MB";
    }

    function formatDate(value) {
        return new Date(value + "Z").toLocaleString();
    }

   
    function getError(xhr, defaultText) {
        if (xhr.responseJSON && xhr.responseJSON.error) {
            return xhr.responseJSON.error;
        }
        return defaultText;
    }

   
    function loadDocuments() {
        $.get(API + "/GetAll", function (documents) {
            $("#docRows").empty();

            $.each(documents, function (i, doc) {
                var row = $("<tr>");
                row.append($("<td>").text(doc.id));
                row.append($("<td>").text(doc.originalFileName));
                row.append($("<td>").text(doc.contentType));
                row.append($("<td>").text(formatSize(doc.fileSize)));
                row.append($("<td>").text(formatDate(doc.uploadedAtUtc)));

                
                var buttons = $("<td>");
                buttons.append($("<button>").addClass("downloadBtn").attr("data-id", doc.id).text("Download"));
                buttons.append(" ");
                buttons.append($("<button>").addClass("deleteBtn").attr("data-id", doc.id).text("Delete"));
                row.append(buttons);

                $("#docRows").append(row);
            });
        }).fail(function (xhr) {
            showMessage(getError(xhr, "Could not load the documents."), "error");
        });
    }

    
    $("#uploadBtn").click(function () {
        var file = $("#fileInput")[0].files[0];

        if (!file) {
            showMessage("Please choose a file first.", "error");
            return;
        }
        if (file.size > MAX_SIZE) {
            showMessage("File is too large. The limit is 10 MB.", "error");
            return;
        }

       
        var formData = new FormData();
        formData.append("file", file);

        $.ajax({
            url: API + "/AddDocument",
            type: "POST",
            data: formData,
            processData: false,   
            contentType: false,   
            success: function (result) {
                showMessage("Uploaded: " + result.originalFileName + " (ID " + result.id + ")", "success");
                $("#fileInput").val("");
                loadDocuments();
            },
            error: function (xhr) {
                showMessage(getError(xhr, "Upload failed."), "error");
            }
        });
    });

   
    $("#docRows").on("click", ".downloadBtn", function () {
        var id = $(this).attr("data-id");
        window.location.href = API + "/Download?id=" + id;
    });


    $("#docRows").on("click", ".deleteBtn", function () {
        var id = $(this).attr("data-id");

        if (!confirm("Delete this document?")) return;

        $.ajax({
            url: API + "/Delete?id=" + id,
            type: "POST",
            success: function () {
                showMessage("Document deleted.", "success");
                loadDocuments();
            },
            error: function (xhr) {
                showMessage(getError(xhr, "Delete failed."), "error");
                loadDocuments();
            }
        });
    });

   
    loadDocuments();
});