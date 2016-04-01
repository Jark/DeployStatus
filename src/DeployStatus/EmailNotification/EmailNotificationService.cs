using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeployStatus.ApiClients;
using DeployStatus.Configuration;
using DeployStatus.Service;
using log4net;

namespace DeployStatus.EmailNotification
{
    internal class EmailNotificationService : IService
    {
        private readonly ILog log;
        private readonly SmtpClient smtpClient;
        private readonly TrelloClient trelloClient;
        private readonly Timer timer;
        private readonly TimeSpan pollAtTime;
        private readonly int reportAfterDaysInColumn;
        private readonly string deployEmailAddress;

        public EmailNotificationService()
        {
            log = LogManager.GetLogger(typeof (EmailNotificationService));
            smtpClient = new SmtpClient();
            var trelloApiConfiguration = DeployStatusSettingsSection.Settings.Trello.AsTrelloApiConfiguration();
            reportAfterDaysInColumn = trelloApiConfiguration.EmailNotificationConfiguration.ReportAfterDaysInColumn;
            trelloClient = new TrelloClient(trelloApiConfiguration);
            deployEmailAddress = trelloApiConfiguration.EmailResolver.GetEmail("deployalready");

            pollAtTime = new TimeSpan(9, 00, 00);

            timer = new Timer(SendEmailsIfNecessary);
        }

        public void Start()
        {
            log.Info("Email notification service started.");
            ScheduleRetryIn(GetDueTime());
        }

        private TimeSpan GetDueTime()
        {
            var now = DateTime.Now;
            var pollDay = now.TimeOfDay <= pollAtTime ? now.Date : now.Date.AddDays(1);
            var dueTime = (pollDay.Add(pollAtTime)) - now;
            return dueTime;
        }

        private async void SendEmailsIfNecessary(object state)
        {
            try
            {
                if (IsWeekend())
                {
                    log.InfoFormat("It's the weekend, let's not bother people right now.");
                }
                else
                {
                    log.InfoFormat("Starting check to see if any cards in the pending deploy column are inactive.");
                    var cardsThatAreInactive = await trelloClient.GetCardsThatAreInactive();
                    var cardsByMember = GetInactiveCardsByMember(cardsThatAreInactive);

                    log.InfoFormat("Check finished, generating emails for: \r\n{0}", string.Join("\r\n", cardsByMember));
                    var emails = cardsByMember.Select(x => new {x.Member, Email = CreateEmail(x) });

                    log.InfoFormat("Finished generating emails, sending them out.");
                    foreach (var email in emails)
                    {
                        await SendEmail(email.Member, email.Email);
                    }
                    log.InfoFormat("Finished sending emails");
                }
            }
            catch(Exception ex)
            {
                log.Error("Error while checking / sending emails.", ex);
                ScheduleRetryIn(TimeSpan.FromMinutes(15));                
                return;
            }

            ScheduleRetryIn(GetDueTime());
        }

        private static bool IsWeekend()
        {
            var now = DateTime.UtcNow;
            return now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday;
        }

        private async Task SendEmail(TrelloMemberInfo member, string email)
        {
            var from = new MailAddress(deployEmailAddress, "Deploy status");
            var to = new MailAddress(member.Email, member.Name);

            var mailMessage = new MailMessage(from, to);
            mailMessage.Body = email;
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = "Card activity notice";

            await smtpClient.SendMailAsync(mailMessage);
        }

        private string CreateEmail(CardsByMember member)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<html>\r\n<body>");
            stringBuilder.AppendLine($"<p>Hi {member.Member.Name},</p>");
            var cardsByColumn = member.Cards.GroupBy(x => x.ListName);
            foreach (var column in cardsByColumn)
            {
                stringBuilder.AppendLine($"<p>The following cards have been in the {column.Key} column for over {reportAfterDaysInColumn} days and it might be a good idea to deploy them:<br/><ul>");
                foreach (var card in column)
                {
                    stringBuilder.AppendLine($"<li><a href=\"{card.Url}\">{card.Name}</a> - last activity was at: {card.LastActivity}</li>");
                }
                stringBuilder.AppendLine("</ul></p>");
            }
            stringBuilder.Append("</body>\r\n</html>");
            return stringBuilder.ToString();
        }

        private void ScheduleRetryIn(TimeSpan dueTime)
        {
            timer.Change(dueTime, TimeSpan.FromMilliseconds(-1));
            log.InfoFormat("Retrying check in {0}.", dueTime);
        }

        private IReadOnlyList<CardsByMember> GetInactiveCardsByMember(IEnumerable<TrelloCardInfo> cardsThatAreInactive)
        {
            return cardsThatAreInactive
                .SelectMany(x => x.Members.Select(y => new { Card = x, Member = y }))
                .GroupBy(x => x.Member, x => x.Card)
                .Select(GetMemberCardsInfo).ToList();
        }

        private CardsByMember GetMemberCardsInfo(IGrouping<TrelloMemberInfo, TrelloCardInfo> arg)
        {
            return new CardsByMember(arg.Key, arg);
        }

        public void Stop()
        {
            log.InfoFormat("Stopping email notification service...");
            timer.Dispose();
            log.InfoFormat("Stopping email notification service stopped.");
        }
    }
}