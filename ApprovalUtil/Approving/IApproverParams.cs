namespace ApprovalUtil.Approving;

public interface IApproverParams
{
    string PathToCode { get; }
    bool Interactive { get; }
}