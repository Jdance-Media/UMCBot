using Discord;
using Discord.Interactions;
using GenHTTP.Modules.IO;
using Microsoft.EntityFrameworkCore;
using Pterodactyl.NET.Objects.V0_7.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMCBot.Models;

namespace UMCBot.Modules;

[Group("application", "Set of commands relating to applications.")]
public class ApplicationCommandModule : InteractionModuleBase
{
    public const ulong ApplicationChannel = 0;

    public const ulong Role = 0;

    public const uint VotesRequired = 4;

    //This is a note put on the test branch.

    [SlashCommand("apply", "Command to start the application process for UMC.")]
    public async Task ApplyCommand()
    {
        await RespondWithModalAsync<ApplicationModal>("umc_application");
    }

    [ModalInteraction("umc_application")]
    public async Task ApplicationModalResponse(ApplicationModal modal)
    {
        await using var dbContext = new UMCBotDbContext();

        var application = dbContext.Applications.Find(Context.User.Id);
        if (application != null)
        {
            await RespondAsync("You already have an application submitted.", ephemeral: true);
            return;
        }
        dbContext.Applications.Add(new Application() { UserId = Context.User.Id, Votes = new ulong[] { } });

        var applicationChannel = (ITextChannel)(await Context.Guild.GetChannelAsync(ApplicationChannel));

        var embed = new EmbedBuilder()
            .WithTitle("New Application")
            .WithColor(Color.DarkGrey)
            .WithDescription($"**User:**\n{Context.User.GlobalName}\n\n**Reason:**\n{modal.Reason}\n\n**Previous Work:**\n{modal.PreviousWork}\n\n**Invite Explanation:**{modal.InviteExplanation}")
            .Build();

        await applicationChannel.SendMessageAsync(embed: embed, components: new ComponentBuilder().WithButton("Approve", $"button_approve:{Context.User.Id}", ButtonStyle.Primary, new Emoji("✔")).Build());

        
    }

    [ComponentInteraction("button_approve:*")]
    public async Task ApplicationApprovalResponse(string id)
    {
        if (!ulong.TryParse(id, out var userId))
        {
            await RespondAsync("Specified application could not be found", ephemeral: true);
            return;
        }
        var user = Context.Guild.GetUserAsync(userId);
        if (user == null)
        {
            await RespondAsync("Specified application could not be found", ephemeral: true);
            return;
        }
        await DeferAsync(true);

        await using var dbContext = new UMCBotDbContext();

        var application = dbContext.Applications.Find(userId);
        if (application == null)
        {
            await RespondAsync("Specified application could not be found", ephemeral: true);
            return;
        }
        
        var votes = application.Votes!.ToList();
        if (votes.Contains(Context.User.Id))
        {
            votes.Remove(Context.User.Id);
            await RespondAsync("Removed your vote.", ephemeral: true);
        }
        else
        {
            votes.Add(Context.User.Id);
            await RespondAsync("Added your vote.", ephemeral: true);
        }
        application.Votes = votes.ToArray();

        await dbContext.SaveChangesAsync();

        await CheckApplication(userId);
        return;
    }

    public async Task CheckApplication(ulong id)
    {
        await using var dbContext = new UMCBotDbContext();

        var application = dbContext.Applications.Find(id);
        if (application == null)
            return;

        if (application.Votes!.Length >= VotesRequired)
        {
            var user = await Context.Guild.GetUserAsync(id);
            if (user != null)
                await user.AddRoleAsync(Role);
        }

        dbContext.Applications.Remove(application);
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

