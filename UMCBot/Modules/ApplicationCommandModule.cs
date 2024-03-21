using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMCBot.Modules;

[Group("application", "Set of commands relating to applications.")]
public class ApplicationCommandModule : InteractionModuleBase
{
    public const ulong ApplicationChannel = 0;

    [SlashCommand("apply", "Command to start the application process for UMC.")]
    public async Task ApplyCommand()
    {
        await RespondWithModalAsync<ApplicationModal>("umc_application");
    } 

    [ModalInteraction("umc_application")]
    public async Task ApplicationModalResponse(ApplicationModal modal)
    {
        var applicationChannel = (ITextChannel)(await Context.Guild.GetChannelAsync(ApplicationChannel));

        var embed = new EmbedBuilder()
            .WithTitle("New Application")
            .WithColor(Color.DarkGrey)
            .WithDescription($"**User:**\n{Context.User.GlobalName}\n\n**Reason:**\n{modal.Reason}\n\n**Previous Work:**\n{modal.PreviousWork}\n\n**Invite Explanation:**{modal.InviteExplanation}")
            .Build();

        await applicationChannel.SendMessageAsync(embed: embed, components: new ComponentBuilder().WithButton("Approve", "button_approve", ButtonStyle.Primary, new Emoji("✔")).Build());
    }

    [ComponentInteraction("button_approve")]
    public async Task ApplicationApprovalResponse()
    {

    }
}

public class ApplicationModal : IModal
{
#nullable disable
    public string Title => "UMC Application";

    //Why do they want to join Unturned Modding Collective
    [RequiredInput(true)]
    [InputLabel("Reason")]
    [ModalTextInput("application_reason", TextInputStyle.Paragraph, "Reason why you are submitting this application. Why are you joining UMC?", maxLength: 500)]
    public string Reason { get; set; }

    [RequiredInput(true)]
    [InputLabel("Previous Work")]
    [ModalTextInput("application_previous_works", TextInputStyle.Paragraph, "Add some links to prior work, that could be maps, mods, and/or plugins.", maxLength: 500)]
    public string PreviousWork { get; set; }

    [RequiredInput(true)]
    [InputLabel("Invite Explanation")]
    [ModalTextInput("application_invite_explanation", TextInputStyle.Paragraph, "How were you invited to this Discord, was it through a particular person?", maxLength: 500)]
    public string InviteExplanation { get; set; }
#nullable enable
}

