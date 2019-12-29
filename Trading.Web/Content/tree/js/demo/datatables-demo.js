// Call the dataTables jQuery plugin
$(document).ready(function() {
    $('#dataTable').DataTable({
        // "scrollY":        "200px",
        // "scrollCollapse": true,
        // "paging":         false,
        "searching": true,
        "lengthChange": false,
        "language": {
            "sSearch": " ",
            "searchPlaceholder": "Quick Search"
        }


  });

  $('#SAUploadList').DataTable({
      // "scrollY":        "200px",
      // "scrollCollapse": true,
      // "paging":         false,
      "searching": true,
      "lengthChange": false,
      "language": {
          "sSearch": "",
          "searchPlaceholder":"Quick Search"
      }
       
         
     
  });
});
