if (typeof (Xms) == "undefined") { Xms = { __namespace: true }; }
Xms.Schema = {
    GetEntities: function (filter, callback) {
        var url = '/api/schema/entity?__r=' + new Date().getTime();
        if (filter) {
            for (x in filter) {
                url += '&' + x + '=' + filter[x];
            }
        }
        Xms.Web.GetJson(url, filter, function (data) {
            callback.call(this, data.content);
        });
    }
    , GetEntityById: function (id, callback) {
        var url = '/api/schema/entity/' + id + '?__r=' + new Date().getTime();
        Xms.Web.GetJson(url, null, function (data) {
            callback(data.content);
        });
    }
    , GetAttributes: function (filter, callback) {
        var url = '/api/schema/attribute?__r=' + new Date().getTime();
        //if (filter) {
        //    for (x in filter) {
        //        url += '&' + x + '=' + filter[x];
        //    }
        //}

        Xms.Web.GetJson(url, filter, function (data) {
            callback(data.content);
        });
    }
    , GetQueryViews: function (entityid, callback, filter) {
        var url = '/customize/QueryView/index?EntityId=' + entityid + '&getall=true&LoadData=true&__r=' + new Date().getTime();
        if (filter) {
            for (x in filter) {
                url += '&' + x + '=' + filter[x];
            }
        }
        Xms.Web.GetJson(url, {}, function (data) {
            callback.call(this, data.content);
        });
    }
    , GetEntityGroups: function (callback) {
        var url = '/api/data/retrieve/all/entitygroup/name/name:asc';
        Xms.Web.GetJson(url, null, function (data) {
            callback(data.content);
        });
    }
    , GetFlowWorks: function (filter, callback) {
        var url = '/xms/customize/flow/index?loaddata=true&getall=true&LoadData=true&__r=' + new Date().getTime();
        if (filter) {
            for (x in filter) {
                url += '&' + x + '=' + filter[x];
            }
        }
        Xms.Web.GetJson(url, {}, function (data) {
            callback.call(this, data.content);
        });
    }
    , GetSystemForms: function (entityid, callback, filter) {
        var url = '/xms/customize/systemform/index?loaddata=true&entityid=' + entityid + '&getall=true&LoadData=true&__r=' + new Date().getTime();
        if (filter) {
            for (x in filter) {
                url += '&' + x + '=' + filter[x];
            }
        }
        Xms.Web.GetJson(url, {}, function (data) {
            callback.call(this, data.content);
        });
    }
    , GetAttributesByEntityId: function (entityid, callback) {
        Xms.Schema.GetAttributes({ getall: true, entityid: entityid }, callback);
    }
    , GetEntityRelations: function (referencingEntityId, referencedEntityId, callback) {
        var url = '/api/schema/relationship/' + (referencingEntityId ? 'GetReferencing' : 'GetReferenced');
        if (referencingEntityId) url += '/' + referencingEntityId;
        if (referencedEntityId) url += '/' + referencedEntityId;
        url += '?__r=' + new Date().getTime();
        Xms.Web.GetJson(url, null, function (data) {
            callback(data.content);
        });
    }
    , GetRelatedEntities: function (entityid, callback) {
        var url = '/api/schema/entity/getmanytoone/' + entityid + '?__r=' + new Date().getTime();
        if (entityid) url += '&entityid' + entityid;
        Xms.Web.GetJson(url, null, function (data) {
            callback(data.content);
        });
    }
    , GetOneToManyEntities: function (entityid, callback) {
        var url = '/api/schema/entity/getonetomany/' + entityid + '?__r=' + new Date().getTime();
        //if (entityid) url += '&entityid' + entityid;
        Xms.Web.GetJson(url, null, function (data) {
            callback(data.content);
        });
    }
    , GetOptionsets: function (filter, callback) {
        var url = '/api/schema/optionset?__r=' + new Date().getTime();
        if (filter) {
            for (x in filter) {
                url += '&' + x + '=' + filter[x];
            }
        }
        Xms.Web.GetJson(url, null, function (data) {
            callback(data.content);
        });
    }
    , GetQueryAttributes: function (queryid, filter, callback) {
        var url = '/api/schema/queryview/getattributes/' + queryid + '?__r=' + new Date().getTime();
        if (filter) {
            for (x in filter) {
                url += '&' + x + '=' + filter[x];
            }
        }
        Xms.Web.GetJson(url, filter, function (data) {
            callback(data.content);
        });
    }
}