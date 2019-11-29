if (typeof (Xms) == "undefined") { Xms = { __namespace: true }; }
Xms.SDK = {
    _serverUrl: function () {
        return ORG_SERVERURL + '/api/data';
    },
    _errorHandler: function (req) {
        return new Error("Error : " +
            req.status + ": " +
            req.statusText + ": " +
            JSON.parse(req.responseText).error.message.value);
    },
    _parameterCheck: function (parameter, message) {
        if ((typeof parameter === "undefined") || parameter === null) {
            throw new Error(message);
        }
    },
    _stringParameterCheck: function (parameter, message) {
        if (typeof parameter != "string") {
            throw new Error(message);
        }
    },
    _callbackParameterCheck: function (callbackParameter, message) {
        if (typeof callbackParameter != "function") {
            throw new Error(message);
        }
    },
    createRecord: function (object, type, successCallback, errorCallback) {
        var jsonEntity = window.JSON.stringify(object);

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: this._serverUrl() + "/createrecord",
            data: jsonEntity,
            beforeSend: function (xhr) {
                //Specifying this header ensures that the results will be returned as JSON.
                xhr.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, xhr) {
                successCallback(data.d);
            },
            error: function (xhr, textStatus, errorThrown) {
                errorCallback(Xms.SDK._errorHandler(xhr));
            }
        });
    },
    retrieveRecord: function (id, type, select, successCallback, errorCallback, async) {
        //是否异步
        var isAsync = true;
        if (typeof async != 'undefined') isAsync = async;

        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: this._serverUrl() + "/retrieve?entity=" + type + "&id=" + id,
            async: isAsync,
            beforeSend: function (xhr) {
                //Specifying this header ensures that the results will be returned as JSON.
                xhr.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, xhr) {
                //JQuery does not provide an opportunity to specify a date reviver so this code
                // parses the xhr.responseText rather than use the data parameter passed by JQuery.
                successCallback(JSON.parse(xhr.responseText, SDK.JQuery._dateReviver).d);
            },
            error: function (xhr, textStatus, errorThrown) {
                errorCallback(Xms.SDK._errorHandler(xhr));
            }
        });
    },
    updateRecord: function (id, object, type, successCallback, errorCallback) {
        var jsonEntity = window.JSON.stringify(object);

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            data: jsonEntity,
            url: this._serverUrl() + "/updaterecord?entity=" + type + "&id=" + id,
            beforeSend: function (xhr) {
                //Specifying this header ensures that the results will be returned as JSON.
                xhr.setRequestHeader("Accept", "application/json");
                //Specify the HTTP method MERGE to update just the changes you are submitting.
                xhr.setRequestHeader("X-HTTP-Method", "MERGE");
            },
            success: function (data, textStatus, xhr) {
                //Nothing is returned to the success function
                successCallback();
            },
            error: function (xhr, textStatus, errorThrown) {
                errorCallback(Xms.SDK._errorHandler(xhr));
            }
        });
    },
    deleteRecord: function (id, type, successCallback, errorCallback) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: this._serverUrl() + "/delete?entity=" + type + "&id=" + id,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON.
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
                //Specify the HTTP method DELETE to perform a delete operation.
                XMLHttpRequest.setRequestHeader("X-HTTP-Method", "DELETE");
            },
            success: function (data, textStatus, xhr) {
                // Nothing is returned to the success function.
                successCallback();
            },
            error: function (xhr, textStatus, errorThrown) {
                errorCallback(Xms.SDK._errorHandler(xhr));
            }
        });
    },
    retrieveMultipleRecords: function (type, options, successCallback, errorCallback, OnComplete, async) {
        //是否异步
        var isAsync = true;
        if (typeof async != 'undefined') isAsync = async;

        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: this._serverUrl() + "/retrieve/multiple?entity=" + type,
            async: isAsync,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON.
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, xhr) {
                if (data && data.d && data.d.results) {
                    successCallback(data);
                    if (data.d.__next != null) {
                        var queryOptions = data.d.__next.substring((SDK.JQuery._ODataPath() + type + "Set").length);
                        Xms.SDK.retrieveMultipleRecords(type, queryOptions, successCallback, errorCallback, OnComplete);
                    }
                    else { OnComplete(); }
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                errorCallback(Xms.SDK._errorHandler(xhr));
            }
        });
    },
    __namespace: true
};