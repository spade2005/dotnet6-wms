namespace mvc_andy.Models.com;

public enum AuthType
{
    Menu = 1,
    Data
};
public enum StatusType
{
    Enable = 0,
    Disable
}

public enum DeleteType
{
    Enable = 0,
    Disable,
    Pending
}

public enum OrderStatusType
{
    Pending = 1,
    Failed,
    Success,
}

public enum StockType
{
    Default = 0,
    Pending,
    Success
}

public enum OrderInOutType
{
    In = 1,
    Out
}
