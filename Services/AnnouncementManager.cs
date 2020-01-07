using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services.Abstract;
using WeebReader.Data.Contexts.Abstract;
using WeebReader.Data.Entities;

namespace Services
{
    public class AnnouncementManager : BaseService
    {
        public AnnouncementManager(BaseContext context) : base(context) { }
        
        public async Task<IEnumerable<Announcement>> GetAnnouncements(int skip, int take) => await Context.Announcements.Skip(skip).Take(take).ToArrayAsync();
        
        public async Task<Announcement> GetAnnouncementById(Guid announcementId) => await Context.Announcements.SingleOrDefaultAsync(announcement => announcement.Id == announcementId);

        public async Task<bool> AddAnnouncement(Announcement announcement)
        {
            if (ValidateAnnouncement(announcement))
                return false;

            announcement.Id = Guid.Empty;
            announcement.CreationDate = DateTime.UtcNow;

            await Context.Announcements.AddAsync(announcement);

            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EditAnnouncement(Announcement announcement)
        {
            if (ValidateAnnouncement(announcement))
                return false;

            var ann = await Context.Announcements.SingleOrDefaultAsync(a => a.Id == announcement.Id);

            if (ann == null)
                return false;

            ann.Name = announcement.Name;
            ann.Content = announcement.Content;
            ann.ReleaseDate = announcement.ReleaseDate;

            Context.Announcements.Update(ann);

            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAnnouncement(Guid announcementId)
        {
            var announcement = await Context.Announcements.SingleOrDefaultAsync(ann => ann.Id == announcementId);

            if (announcement == null)
                return false;

            Context.Announcements.Remove(announcement);

            return await Context.SaveChangesAsync() > 0;
        }

        private static bool ValidateAnnouncement(Announcement announcement)
        {
            if (announcement == null)
                return false;

            var titleExists = !string.IsNullOrWhiteSpace(announcement.Name);
            var contentExists = !string.IsNullOrWhiteSpace(announcement.Content);

            return titleExists && contentExists;
        }
    }
}