function Retrivedata(url, arguments) {

    var dt;

    $.ajax(
        {
            async : false,
            type: 'GET',
            url: url,
            data: arguments,
            success:
                function (result) {
                    dt = result;
                },
            error:
                function (response) {
                    alert("Error: " + response);
                }
        });

    return dt;
}