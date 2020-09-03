// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    var x1 = $('#PlaceHolderHere');
    $('button[data-toggle="ajax-modal"]').click(function (event) {
        var url = $(this).data('url');
        var decodeUrl = decodeURIComponent(url);
        $.get(decodeUrl).done(function (data) {
            x1.html(data);
            x1.find('.modal').modal('show');
        })
    })
})

window.onload = function () {
    document.getElementById("download")
        .addEventListener("click", () => {
            const invoice = this.document.getElementById("report");
            console.log(invoice);
            console.log(window);
            var opt = {
                
                filename: Date.now("yyyyMMdd")+'Report.pdf',
                image: { type: 'jpeg', quality: 0.98 },
                html2canvas: { scale: 2 },
                jsPDF: { unit: 'in', format: 'letter', orientation: 'portrait' }
            };
            html2pdf().from(invoice).set(opt).save();
        })
}