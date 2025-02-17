﻿
namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Shared;
    using Microsoft.ApplicationInsights;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;
    using System.Security.Claims;

    [Route("api/users")]
    public class UsersController : SecureController
    {
        private readonly IUserManager userManager;

        public UsersController(IUserManager userManager)
            : base()
        {
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var result = await userManager.GetAsync(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet("getUserByUserName/{userName}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetUser(string userName, CancellationToken cancellationToken)
        {
            var user = await userManager.GetUserByUserNameAsync(userName, cancellationToken).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("verifyunique/{userId}/{userName}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> VerifyUnique(Guid userId, string userName, CancellationToken cancellationToken)
        {
            var user = await userManager.GetUserByUserNameAsync(userName, cancellationToken).ConfigureAwait(false);

            if (user == null)
            {
                return Ok();
            }

            if (user.Id != userId)
            {
                return Conflict();
            }

            return Ok();
        }

        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetUserByInternalId(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await userManager.GetUserByInternalIdAsync(id, cancellationToken).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut()]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> PutUser(User user, CancellationToken cancellationToken)
        {
            try
            {
                var updatedUser = await userManager.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
                TelemetryClient.TrackEvent("UpdateUser");
                return Ok(updatedUser);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await userManager.UserExistsAsync(user.Id, cancellationToken).ConfigureAwait(false))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> PostUser(User user, CancellationToken cancellationToken)
        {
            User originalUser;

            if ((originalUser = await userManager.UserExistsAsync(user.NameIdentifier, cancellationToken).ConfigureAwait(false)) != null)
            {
                if (!ValidateUser(originalUser.NameIdentifier))
                {
                    return Forbid();
                }

                originalUser.Email = user.Email;
                originalUser.SourceSystemUserName = user.SourceSystemUserName;

                var updatedUser = await userManager.UpdateAsync(originalUser, cancellationToken).ConfigureAwait(false);
                TelemetryClient.TrackEvent("UpdateUser");

                return Ok(updatedUser);
            }

            if (string.IsNullOrEmpty(user.UserName))
            {
                // On insert we need a random user name to avoid duplicates, but we don't want to show the full email address ever, so take a subset
                // of their email and then add a random number to the end.
                Random rnd = new();
                var userNum = rnd.Next(100, 999).ToString();
                var first = user.Email.Split("@")[0];
                user.UserName = first.Substring(0, Math.Min(first.Length - 1, 8)) + userNum;
            }

            var newUser = await userManager.AddAsync(user, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent("AddUser");

            return CreatedAtAction(nameof(GetUserByInternalId), new { id = newUser.Id }, newUser);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
        {
            await userManager.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeleteUser));

            return Ok(id);
        }

        private bool ValidateUser(string userId)
        {
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return userId == nameIdentifier;
        }
    }
}
