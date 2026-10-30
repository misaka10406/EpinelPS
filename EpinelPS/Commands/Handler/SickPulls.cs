using EpinelPS.Database;
using EpinelPS.Commands.Core;
using EpinelPS.Commands.Binding;

namespace EpinelPS.Commands.Handler;

public class SickPullsParameter : ICommandParameters
{
    static ParameterDescriptor[] ICommandParameters.Descriptors => [];
}

public class SickPullsHandler(IExecutionContext context) : BaseHandler<SickPullsParameter>(context)
{
    public override string Name => "sick-pulls";
    public override string Description => "Toggle equal chances for all characters to be pulled";

    protected async override Task<HandleResult> ExecuteAsync(SickPullsParameter parameters)
    {
        if (context.SelectedUser == null)
            return new HandleResult(false, "No user selected");

        var curSickPulls = JsonDb.IsSickPulls(context.SelectedUser);
        var newVal = !curSickPulls;
        context.SelectedUser.sickpulls = newVal;
        JsonDb.Save();
        return new HandleResult(true, newVal
            ? "已开启所有角色拥有相同的抽取概率"
            : "已关闭所有角色拥有相同的抽取概率");
    }
}
