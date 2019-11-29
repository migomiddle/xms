if (typeof (Xms) == "undefined") { Xms = { __namespace: true }; }
Xms.Fetch = function () { };
//Xms.Fetch.LogicalOperator = function () { };
Xms.Fetch.LogicalOperator = {
    And: 0,
    Or: 1
};
//Xms.Fetch.ConditionOperator = function () { };
Xms.Fetch.ConditionOperator = {
    Equal: 0,
    NotEqual: 1,
    GreaterThan: 2,
    LessThan: 3,
    GreaterEqual: 4,
    LessEqual: 5,
    Like: 6,
    NotLike: 7,
    In: 8,
    NotIn: 9,
    Between: 10,
    NotBetween: 11,
    Null: 12,
    NotNull: 13,
    Yesterday: 14,
    Today: 15,
    Tomorrow: 16,
    Last7Days: 17,
    Next7Days: 18,
    LastWeek: 19,
    ThisWeek: 20,
    NextWeek: 21,
    LastMonth: 22,
    ThisMonth: 23,
    NextMonth: 24,
    On: 25,
    OnOrBefore: 26,
    OnOrAfter: 27,
    Before: 28,
    After: 29,
    LastYear: 30,
    ThisYear: 31,
    NextYear: 32,
    LastXHours: 33,
    NextXHours: 34,
    LastXDays: 35,
    NextXDays: 36,
    LastXWeeks: 37,
    NextXWeeks: 38,
    LastXMonths: 39,
    NextXMonths: 40,
    LastXYears: 41,
    NextXYears: 42,
    EqualUserId: 43,
    NotEqualUserId: 44,
    EqualBusinessId: 45,
    NotEqualBusinessId: 46,
    ChildOf: 47,
    Mask: 48,
    NotMask: 49,
    MasksSelect: 50,
    Contains: 51,
    DoesNotContain: 52,
    EqualUserLanguage: 53,
    NotOn: 54,
    OlderThanXMonths: 55,
    BeginsWith: 56,
    DoesNotBeginWith: 57,
    EndsWith: 58,
    DoesNotEndWith: 59,
    ThisFiscalYear: 60,
    ThisFiscalPeriod: 61,
    NextFiscalYear: 62,
    NextFiscalPeriod: 63,
    LastFiscalYear: 64,
    LastFiscalPeriod: 65,
    LastXFiscalYears: 66,
    LastXFiscalPeriods: 67,
    NextXFiscalYears: 68,
    NextXFiscalPeriods: 69,
    InFiscalYear: 70,
    InFiscalPeriod: 71,
    InFiscalPeriodAndYear: 72,
    InOrBeforeFiscalPeriodAndYear: 73,
    InOrAfterFiscalPeriodAndYear: 74,
    EqualUserTeams: 75,
    EqualOrganizationId: 76,
    NotEqualOrganizationId: 77,
    OnOrBeforeToday: 78,
    OnOrAfterToday: 79,
    BeforeToday: 80,
    AfterToday: 81,
    OlderThanXYears: 82,
    OlderThanXDays: 83,
    AfterXYears: 84,
    AfterXMonths: 85,
    AfterXDays: 86
};
Xms.Fetch.ConditionOperatorLabel = {
    Equal: '等于',
    NotEqual: '不等于',
    GreaterThan: '大于',
    LessThan: '小于',
    GreaterEqual: '大于等于',
    LessEqual: '小于等于',
    Like: '包含',
    NotLike: '不包含',
    In: '包含',
    NotIn: '不包含',
    Between: '介于',
    NotBetween: '不介于',
    Null: '不包含数据',
    NotNull: '包含数据',
    Yesterday: '昨天',
    Today: '今天',
    Tomorrow: '明天',
    Last7Days: '往前7天',
    Next7Days: '往后7天',
    LastWeek: '上周',
    ThisWeek: '本周',
    NextWeek: '下周',
    LastMonth: '上个月',
    ThisMonth: '本月',
    NextMonth: '下个月',
    On: '等于',
    OnOrBefore: '早于(包含当天)',
    OnOrAfter: '晚于(包含当天)',
    Before: '早于',
    After: '晚于',
    LastYear: '去年',
    ThisYear: '今年',
    NextYear: '明年',
    LastXHours: '往前X小时',
    NextXHours: '往后X小时',
    LastXDays: '往前X天',
    NextXDays: '往后X天',
    LastXWeeks: '往前X周',
    NextXWeeks: '往后X周',
    LastXMonths: '往前X月',
    NextXMonths: '往后X月',
    LastXYears: '往前X年',
    NextXYears: '往后X年',
    EqualUserId: '等于当前用户',
    NotEqualUserId: '不等于当前用户',
    EqualBusinessId: '等于当前部门',
    NotEqualBusinessId: '不等于当前部门',
    ChildOf: '下属',
    Mask: '',
    NotMask: '',
    MasksSelect: '',
    Contains: '包含',
    DoesNotContain: '不包含',
    EqualUserLanguage: '等于当前用户语言',
    NotOn: '不等于',
    OlderThanXMonths: 'X个月以前',
    BeginsWith: '开头等于',
    DoesNotBeginWith: '开头不等于',
    EndsWith: '结尾等于',
    DoesNotEndWith: '结尾不等于',
    ThisFiscalYear: '当前会计年度',
    ThisFiscalPeriod: '当前会计期间',
    NextFiscalYear: '下一会计年度',
    NextFiscalPeriod: '下一会计期间',
    LastFiscalYear: '上一会计年度',
    LastFiscalPeriod: '上一会计期间',
    LastXFiscalYears: '过去X个会计年度',
    LastXFiscalPeriods: '过去X个会计期间',
    NextXFiscalYears: '往后X个会计年度',
    NextXFiscalPeriods: '往后X个会计期间',
    InFiscalYear: '在会计年度内',
    InFiscalPeriod: '在会计期间内',
    InFiscalPeriodAndYear: '在会计期间及年度内',
    InOrBeforeFiscalPeriodAndYear: '在会计期间及年度之前',
    InOrAfterFiscalPeriodAndYear: '在会计期间及年度之后',
    EqualUserTeams: '当前用户团队',
    EqualOrganizationId: '等于当前组织',
    NotEqualOrganizationId: '不等于当前组织',
    OnOrBeforeToday: '今天之前(包含当天)',
    OnOrAfterToday: '今天之后(包含当天)',
    BeforeToday: '今天之前',
    AfterToday: '今天之后',
    OlderThanXYears: 'X年以前',
    OlderThanXDays: 'X天以前',
    AfterXYears: 'X年以后',
    AfterXMonths: 'X个月以后',
    AfterXDays: 'X天以后'
};
//models
Xms.Fetch.FilterExpression = function (operator) {
    var self = new Object();
    self.FilterOperator = operator || Xms.Fetch.LogicalOperator.And;
    self.Conditions = [];
    self.Filters = [];
    return self;
};
Xms.Fetch.ConditionExpression = function () {
    var self = new Object();
    self.AttributeName = '';
    self.Operator = Xms.Fetch.ConditionOperator.Equal;
    self.Values = [];
    return self;
};

//类型比较符
Xms.Fetch.ConditionOperator.CommonOperators = [];
Xms.Fetch.ConditionOperator.CommonOperators.push(["Equal", Xms.Fetch.ConditionOperator.Equal, Xms.Fetch.ConditionOperatorLabel.Equal]);
Xms.Fetch.ConditionOperator.CommonOperators.push(["NotEqual", Xms.Fetch.ConditionOperator.NotEqual, Xms.Fetch.ConditionOperatorLabel.NotEqual]);
Xms.Fetch.ConditionOperator.CommonOperators.push(["NotNull", Xms.Fetch.ConditionOperator.NotNull, Xms.Fetch.ConditionOperatorLabel.NotNull]);
Xms.Fetch.ConditionOperator.CommonOperators.push(["Null", Xms.Fetch.ConditionOperator.Null, Xms.Fetch.ConditionOperatorLabel.Null]);

Xms.Fetch.ConditionOperator.StringOperators = Xms.Fetch.ConditionOperator.CommonOperators.concat();
Xms.Fetch.ConditionOperator.StringOperators.push(["Like", Xms.Fetch.ConditionOperator.Like, Xms.Fetch.ConditionOperatorLabel.Like]);
Xms.Fetch.ConditionOperator.StringOperators.push(["NotLike", Xms.Fetch.ConditionOperator.NotLike, Xms.Fetch.ConditionOperatorLabel.NotLike]);
Xms.Fetch.ConditionOperator.StringOperators.push(["BeginsWith", Xms.Fetch.ConditionOperator.BeginsWith, Xms.Fetch.ConditionOperatorLabel.BeginsWith]);
Xms.Fetch.ConditionOperator.StringOperators.push(["DoesNotBeginWith", Xms.Fetch.ConditionOperator.DoesNotBeginWith, Xms.Fetch.ConditionOperatorLabel.DoesNotBeginWith]);
Xms.Fetch.ConditionOperator.StringOperators.push(["EndsWith", Xms.Fetch.ConditionOperator.EndsWith, Xms.Fetch.ConditionOperatorLabel.EndsWith]);
Xms.Fetch.ConditionOperator.StringOperators.push(["DoesNotEndWith", Xms.Fetch.ConditionOperator.DoesNotEndWith, Xms.Fetch.ConditionOperatorLabel.DoesNotEndWith]);

Xms.Fetch.ConditionOperator.NumberOperators = Xms.Fetch.ConditionOperator.CommonOperators.concat();
Xms.Fetch.ConditionOperator.NumberOperators.push(["GreaterThan", Xms.Fetch.ConditionOperator.GreaterThan, Xms.Fetch.ConditionOperatorLabel.GreaterThan]);
Xms.Fetch.ConditionOperator.NumberOperators.push(["LessThan", Xms.Fetch.ConditionOperator.LessThan, Xms.Fetch.ConditionOperatorLabel.LessThan]);
Xms.Fetch.ConditionOperator.NumberOperators.push(["GreaterEqual", Xms.Fetch.ConditionOperator.GreaterEqual, Xms.Fetch.ConditionOperatorLabel.GreaterEqual]);
Xms.Fetch.ConditionOperator.NumberOperators.push(["LessEqual", Xms.Fetch.ConditionOperator.LessEqual, Xms.Fetch.ConditionOperatorLabel.LessEqual]);

Xms.Fetch.ConditionOperator.DateTimeOperators = Xms.Fetch.ConditionOperator.CommonOperators.concat();
//Xms.Fetch.ConditionOperator.DateTimeOperators.push(["GreaterThan", Xms.Fetch.ConditionOperator.GreaterThan, Xms.Fetch.ConditionOperatorLabel.GreaterThan]);
//Xms.Fetch.ConditionOperator.DateTimeOperators.push(["LessThan", Xms.Fetch.ConditionOperator.LessThan, Xms.Fetch.ConditionOperatorLabel.LessThan]);
//Xms.Fetch.ConditionOperator.DateTimeOperators.push(["GreaterEqual", Xms.Fetch.ConditionOperator.GreaterEqual, Xms.Fetch.ConditionOperatorLabel.GreaterEqual]);
//Xms.Fetch.ConditionOperator.DateTimeOperators.push(["LessEqual", Xms.Fetch.ConditionOperator.LessEqual, Xms.Fetch.ConditionOperatorLabel.LessEqual]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["On", Xms.Fetch.ConditionOperator.On, Xms.Fetch.ConditionOperatorLabel.On]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["Last7Days", Xms.Fetch.ConditionOperator.Last7Days, Xms.Fetch.ConditionOperatorLabel.Last7Days]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["LastWeek", Xms.Fetch.ConditionOperator.LastWeek, Xms.Fetch.ConditionOperatorLabel.LastWeek]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["LastMonth", Xms.Fetch.ConditionOperator.LastMonth, Xms.Fetch.ConditionOperatorLabel.LastMonth]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["LastYear", Xms.Fetch.ConditionOperator.LastYear, Xms.Fetch.ConditionOperatorLabel.LastYear]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["LastXHours", Xms.Fetch.ConditionOperator.LastXHours, Xms.Fetch.ConditionOperatorLabel.LastXHours]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["LastXDays", Xms.Fetch.ConditionOperator.LastXDays, Xms.Fetch.ConditionOperatorLabel.LastXDays]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["LastXWeeks", Xms.Fetch.ConditionOperator.LastXWeeks, Xms.Fetch.ConditionOperatorLabel.LastXWeeks]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["LastXMonths", Xms.Fetch.ConditionOperator.LastXMonths, Xms.Fetch.ConditionOperatorLabel.LastXMonths]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["LastXYears", Xms.Fetch.ConditionOperator.LastXYears, Xms.Fetch.ConditionOperatorLabel.LastXYears]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["Next7Days", Xms.Fetch.ConditionOperator.Next7Days, Xms.Fetch.ConditionOperatorLabel.Next7Days]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["NextWeek", Xms.Fetch.ConditionOperator.NextWeek, Xms.Fetch.ConditionOperatorLabel.NextWeek]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["NextMonth", Xms.Fetch.ConditionOperator.NextMonth, Xms.Fetch.ConditionOperatorLabel.NextMonth]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["NextYear", Xms.Fetch.ConditionOperator.NextYear, Xms.Fetch.ConditionOperatorLabel.NextYear]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["NextXHours", Xms.Fetch.ConditionOperator.NextXHours, Xms.Fetch.ConditionOperatorLabel.NextXHours]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["NextXDays", Xms.Fetch.ConditionOperator.NextXDays, Xms.Fetch.ConditionOperatorLabel.NextXDays]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["NextXWeeks", Xms.Fetch.ConditionOperator.NextXWeeks, Xms.Fetch.ConditionOperatorLabel.NextXWeeks]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["NextXMonths", Xms.Fetch.ConditionOperator.NextXMonths, Xms.Fetch.ConditionOperatorLabel.NextXMonths]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["NextXYears", Xms.Fetch.ConditionOperator.NextXYears, Xms.Fetch.ConditionOperatorLabel.NextXYears]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["OlderThanXYears", Xms.Fetch.ConditionOperator.OlderThanXYears, Xms.Fetch.ConditionOperatorLabel.OlderThanXYears]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["OlderThanXMonths", Xms.Fetch.ConditionOperator.OlderThanXMonths, Xms.Fetch.ConditionOperatorLabel.OlderThanXMonths]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["OlderThanXDays", Xms.Fetch.ConditionOperator.OlderThanXDays, Xms.Fetch.ConditionOperatorLabel.OlderThanXDays]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["AfterXYears", Xms.Fetch.ConditionOperator.AfterXYears, Xms.Fetch.ConditionOperatorLabel.AfterXYears]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["AfterXMonths", Xms.Fetch.ConditionOperator.AfterXMonths, Xms.Fetch.ConditionOperatorLabel.AfterXMonths]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["AfterXDays", Xms.Fetch.ConditionOperator.AfterXDays, Xms.Fetch.ConditionOperatorLabel.AfterXDays]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["OnOrBefore", Xms.Fetch.ConditionOperator.OnOrBefore, Xms.Fetch.ConditionOperatorLabel.OnOrBefore]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["OnOrAfter", Xms.Fetch.ConditionOperator.OnOrAfter, Xms.Fetch.ConditionOperatorLabel.OnOrAfter]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["Before", Xms.Fetch.ConditionOperator.Before, Xms.Fetch.ConditionOperatorLabel.Before]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["After", Xms.Fetch.ConditionOperator.After, Xms.Fetch.ConditionOperatorLabel.After]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["ThisWeek", Xms.Fetch.ConditionOperator.ThisWeek, Xms.Fetch.ConditionOperatorLabel.ThisWeek]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["ThisMonth", Xms.Fetch.ConditionOperator.ThisMonth, Xms.Fetch.ConditionOperatorLabel.ThisMonth]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["ThisYear", Xms.Fetch.ConditionOperator.ThisYear, Xms.Fetch.ConditionOperatorLabel.ThisYear]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["Today", Xms.Fetch.ConditionOperator.Today, Xms.Fetch.ConditionOperatorLabel.Today]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["Tomorrow", Xms.Fetch.ConditionOperator.Tomorrow, Xms.Fetch.ConditionOperatorLabel.Tomorrow]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["Yesterday", Xms.Fetch.ConditionOperator.Yesterday, Xms.Fetch.ConditionOperatorLabel.Yesterday]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["OnOrBeforeToday", Xms.Fetch.ConditionOperator.OnOrBeforeToday, Xms.Fetch.ConditionOperatorLabel.OnOrBeforeToday]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["OnOrAfterToday", Xms.Fetch.ConditionOperator.OnOrAfterToday, Xms.Fetch.ConditionOperatorLabel.OnOrAfterToday]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["BeforeToday", Xms.Fetch.ConditionOperator.BeforeToday, Xms.Fetch.ConditionOperatorLabel.BeforeToday]);
Xms.Fetch.ConditionOperator.DateTimeOperators.push(["AfterToday", Xms.Fetch.ConditionOperator.AfterToday, Xms.Fetch.ConditionOperatorLabel.AfterToday]);

Xms.Fetch.ConditionOperator.LookUpOperators = Xms.Fetch.ConditionOperator.CommonOperators.concat();
Xms.Fetch.ConditionOperator.LookUpOperators.push(["Like", Xms.Fetch.ConditionOperator.Like, Xms.Fetch.ConditionOperatorLabel.Like]);
Xms.Fetch.ConditionOperator.LookUpOperators.push(["NotLike", Xms.Fetch.ConditionOperator.NotLike, Xms.Fetch.ConditionOperatorLabel.NotLike]);
Xms.Fetch.ConditionOperator.LookUpOperators.push(["BeginsWith", Xms.Fetch.ConditionOperator.BeginsWith, Xms.Fetch.ConditionOperatorLabel.BeginsWith]);
Xms.Fetch.ConditionOperator.LookUpOperators.push(["DoesNotBeginWith", Xms.Fetch.ConditionOperator.DoesNotBeginWith, Xms.Fetch.ConditionOperatorLabel.DoesNotBeginWith]);
Xms.Fetch.ConditionOperator.LookUpOperators.push(["EndsWith", Xms.Fetch.ConditionOperator.EndsWith, Xms.Fetch.ConditionOperatorLabel.EndsWith]);
Xms.Fetch.ConditionOperator.LookUpOperators.push(["DoesNotEndWith", Xms.Fetch.ConditionOperator.DoesNotEndWith, Xms.Fetch.ConditionOperatorLabel.DoesNotEndWith]);

Xms.Fetch.ConditionOperator.OwnerOperators = Xms.Fetch.ConditionOperator.CommonOperators.concat();
Xms.Fetch.ConditionOperator.OwnerOperators.push(["Like", Xms.Fetch.ConditionOperator.Like, Xms.Fetch.ConditionOperatorLabel.Like]);
Xms.Fetch.ConditionOperator.OwnerOperators.push(["NotLike", Xms.Fetch.ConditionOperator.NotLike, Xms.Fetch.ConditionOperatorLabel.NotLike]);
Xms.Fetch.ConditionOperator.OwnerOperators.push(["BeginsWith", Xms.Fetch.ConditionOperator.BeginsWith, Xms.Fetch.ConditionOperatorLabel.BeginsWith]);
Xms.Fetch.ConditionOperator.OwnerOperators.push(["DoesNotBeginWith", Xms.Fetch.ConditionOperator.DoesNotBeginWith, Xms.Fetch.ConditionOperatorLabel.DoesNotBeginWith]);
Xms.Fetch.ConditionOperator.OwnerOperators.push(["EndsWith", Xms.Fetch.ConditionOperator.EndsWith, Xms.Fetch.ConditionOperatorLabel.EndsWith]);
Xms.Fetch.ConditionOperator.OwnerOperators.push(["DoesNotEndWith", Xms.Fetch.ConditionOperator.DoesNotEndWith, Xms.Fetch.ConditionOperatorLabel.DoesNotEndWith]);
Xms.Fetch.ConditionOperator.OwnerOperators.push(["EqualUserId", Xms.Fetch.ConditionOperator.EqualUserId, Xms.Fetch.ConditionOperatorLabel.EqualUserId]);
Xms.Fetch.ConditionOperator.OwnerOperators.push(["NotEqualUserId", Xms.Fetch.ConditionOperator.NotEqualUserId, Xms.Fetch.ConditionOperatorLabel.NotEqualUserId]);

Xms.Fetch.ConditionOperator.SystemUserOperators = Xms.Fetch.ConditionOperator.OwnerOperators.concat();

Xms.Fetch.ConditionOperator.BusinessUnitOperators = Xms.Fetch.ConditionOperator.LookUpOperators.concat();
//Xms.Fetch.ConditionOperator.BusinessUnitOperators.push(["In", Xms.Fetch.ConditionOperator.In, Xms.Fetch.ConditionOperatorLabel.In]);
//Xms.Fetch.ConditionOperator.BusinessUnitOperators.push(["NotIn", Xms.Fetch.ConditionOperator.NotIn, Xms.Fetch.ConditionOperatorLabel.NotIn]);
//Xms.Fetch.ConditionOperator.BusinessUnitOperators.push(["BeginsWith", Xms.Fetch.ConditionOperator.BeginsWith, Xms.Fetch.ConditionOperatorLabel.BeginsWith]);
//Xms.Fetch.ConditionOperator.BusinessUnitOperators.push(["DoesNotBeginWith", Xms.Fetch.ConditionOperator.DoesNotBeginWith, Xms.Fetch.ConditionOperatorLabel.DoesNotBeginWith]);
//Xms.Fetch.ConditionOperator.BusinessUnitOperators.push(["EndsWith", Xms.Fetch.ConditionOperator.EndsWith, Xms.Fetch.ConditionOperatorLabel.EndsWith]);
//Xms.Fetch.ConditionOperator.BusinessUnitOperators.push(["DoesNotEndWith", Xms.Fetch.ConditionOperator.DoesNotEndWith, Xms.Fetch.ConditionOperatorLabel.DoesNotEndWith]);
Xms.Fetch.ConditionOperator.BusinessUnitOperators.push(["EqualBusinessId", Xms.Fetch.ConditionOperator.EqualBusinessId, Xms.Fetch.ConditionOperatorLabel.EqualBusinessId]);
Xms.Fetch.ConditionOperator.BusinessUnitOperators.push(["NotEqualBusinessId", Xms.Fetch.ConditionOperator.NotEqualBusinessId, Xms.Fetch.ConditionOperatorLabel.NotEqualBusinessId]);

Xms.Fetch.ConditionOperator.OrganizationOperators = Xms.Fetch.ConditionOperator.LookUpOperators.concat();
Xms.Fetch.ConditionOperator.OrganizationOperators.push(["EqualOrganizationId", Xms.Fetch.ConditionOperator.EqualOrganizationId, Xms.Fetch.ConditionOperatorLabel.EqualOrganizationId]);
Xms.Fetch.ConditionOperator.OrganizationOperators.push(["NotEqualOrganizationId", Xms.Fetch.ConditionOperator.NotEqualOrganizationId, Xms.Fetch.ConditionOperatorLabel.NotEqualOrganizationId]);

Xms.Fetch.ConditionOperator.PickListOperators = Xms.Fetch.ConditionOperator.CommonOperators.concat();
Xms.Fetch.ConditionOperator.PickListOperators.push(["In", Xms.Fetch.ConditionOperator.In, Xms.Fetch.ConditionOperatorLabel.In]);
Xms.Fetch.ConditionOperator.PickListOperators.push(["NotIn", Xms.Fetch.ConditionOperator.NotIn, Xms.Fetch.ConditionOperatorLabel.NotIn]);
//Xms.Fetch.ConditionOperator.PickListOperators.push(["GreaterThan", Xms.Fetch.ConditionOperator.GreaterThan, "大于"]);
//Xms.Fetch.ConditionOperator.PickListOperators.push(["LessThan", Xms.Fetch.ConditionOperator.LessThan, "小于"]);
//Xms.Fetch.ConditionOperator.PickListOperators.push(["GreaterEqual", Xms.Fetch.ConditionOperator.GreaterEqual, "大于等于"]);
//Xms.Fetch.ConditionOperator.PickListOperators.push(["LessEqual", Xms.Fetch.ConditionOperator.LessEqual, "小于等于"]);
//Xms.Fetch.ConditionOperator.PickListOperators.push(["BeginsWith", Xms.Fetch.ConditionOperator.BeginsWith, "开头等于"]);
//Xms.Fetch.ConditionOperator.PickListOperators.push(["DoesNotBeginWith", Xms.Fetch.ConditionOperator.DoesNotBeginWith, "开头不等于"]);
//Xms.Fetch.ConditionOperator.PickListOperators.push(["EndsWith", Xms.Fetch.ConditionOperator.EndsWith, "结尾等于"]);
//Xms.Fetch.ConditionOperator.PickListOperators.push(["DoesNotEndWith", Xms.Fetch.ConditionOperator.DoesNotEndWith, "结尾不等于"]);

Xms.Fetch.ConditionOperators = [];
Xms.Fetch.ConditionOperators["nvarchar"] = Xms.Fetch.ConditionOperator.StringOperators.concat();
Xms.Fetch.ConditionOperators["datetime"] = Xms.Fetch.ConditionOperator.DateTimeOperators.concat();
Xms.Fetch.ConditionOperators["lookup"] = Xms.Fetch.ConditionOperator.LookUpOperators.concat();
Xms.Fetch.ConditionOperators["owner"] = Xms.Fetch.ConditionOperator.OwnerOperators.concat();
Xms.Fetch.ConditionOperators["picklist"] = Xms.Fetch.ConditionOperator.PickListOperators.concat();
Xms.Fetch.ConditionOperators["bit"] = Xms.Fetch.ConditionOperator.PickListOperators.concat();
Xms.Fetch.ConditionOperators["int"] = Xms.Fetch.ConditionOperator.NumberOperators.concat();
Xms.Fetch.ConditionOperators["money"] = Xms.Fetch.ConditionOperator.NumberOperators.concat();
Xms.Fetch.ConditionOperators["float"] = Xms.Fetch.ConditionOperator.NumberOperators.concat();
Xms.Fetch.ConditionOperators["decimal"] = Xms.Fetch.ConditionOperator.NumberOperators.concat();
Xms.Fetch.ConditionOperators["state"] = Xms.Fetch.ConditionOperator.PickListOperators.concat();
Xms.Fetch.ConditionOperators["businessunit"] = Xms.Fetch.ConditionOperator.OwnerOperators.concat();
Xms.Fetch.ConditionOperators["systemuser"] = Xms.Fetch.ConditionOperator.SystemUserOperators.concat();
Xms.Fetch.ConditionOperators["organization"] = Xms.Fetch.ConditionOperator.OrganizationOperators.concat();

//根据数字获取操作符名字
Xms.Fetch.getFilterName = function (number) {
    var res = '';
    for (var i in Xms.Fetch.ConditionOperator) {
        if (Xms.Fetch.ConditionOperator.hasOwnProperty(i)) {
            if (number == Xms.Fetch.ConditionOperator[i]) {
                res = i;
                break;
            }
        }
    }
    return res;
}