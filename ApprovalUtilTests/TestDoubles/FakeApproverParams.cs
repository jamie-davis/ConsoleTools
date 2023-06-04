using ApprovalUtil.Approving;

namespace ApprovalUtilTests.TestDoubles;

public class FakeApproverParams : IApproverParams
{
    #region Implementation of IApproverParams

    public string PathToCode { get; set; }

    public bool Interactive { get; set; }

    #endregion
}