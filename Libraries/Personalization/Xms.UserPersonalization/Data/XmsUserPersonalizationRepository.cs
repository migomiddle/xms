using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Infrastructure.Utility;
using Xms.UserPersonalization.Domain;

namespace Xms.UserPersonalization.Data
{
    public class XmsUserPersonalizationRepository : IUserPersonalizationRepository
    {
        private readonly Xms.IUserPersonalizationRepository _userPersonalizationRepository;

        public XmsUserPersonalizationRepository(Xms.IUserPersonalizationRepository userPersonalizationRepository)
        {
            _userPersonalizationRepository = userPersonalizationRepository;
        }

        public List<Domain.UserPersonalization> Get(Guid ownerId)
        {
            return Convert(_userPersonalizationRepository.Query(x => x.OwnerId == ownerId));
        }

        public Domain.UserPersonalization GetByName(Guid ownerId, string name)
        {
            return Convert(_userPersonalizationRepository.Find(x => x.OwnerId == ownerId && x.Name == name));
        }

        public bool Set(Domain.UserPersonalization userPersonalization)
        {
            UserCustomization entity = Convert(userPersonalization);
            if (_userPersonalizationRepository.Exists(x => x.OwnerId == entity.OwnerId))
            {
                return _userPersonalizationRepository.Update(entity);
            }
            else
            {
                return _userPersonalizationRepository.Create(entity);
            }
        }

        public bool Delete(Guid ownerId)
        {
            var list = _userPersonalizationRepository.Query(x => x.OwnerId == ownerId);
            if (list.NotEmpty())
            {
                return _userPersonalizationRepository.DeleteMany(list.Select(x => x.UserCustomizationId));
            }
            return true;
        }

        public bool Delete(Guid ownerId, string name)
        {
            var entity = _userPersonalizationRepository.Find(x => x.OwnerId == ownerId && x.Name == name);
            if (entity != null)
            {
                return _userPersonalizationRepository.DeleteById(entity.UserCustomizationId);
            }
            return true;
        }

        public bool DeleteById(Guid id)
        {
            return _userPersonalizationRepository.DeleteById(id);
        }

        public Domain.UserPersonalization Convert(UserCustomization entity)
        {
            return new Domain.UserPersonalization()
            {
                Id = entity.UserCustomizationId,
                Name = entity.Name,
                Value = entity.Value,
                OwnerId = entity.OwnerId
            };
        }

        public List<Domain.UserPersonalization> Convert(List<UserCustomization> entity)
        {
            List<Domain.UserPersonalization> list = new List<Domain.UserPersonalization>();
            foreach (var item in entity)
            {
                list.Add(Convert(item));
            }
            return list;
        }

        public UserCustomization Convert(Domain.UserPersonalization userPersonalization)
        {
            return new Domain.UserCustomization()
            {
                UserCustomizationId = userPersonalization.Id,
                Name = userPersonalization.Name,
                Value = userPersonalization.Value,
                OwnerId = userPersonalization.OwnerId
            };
        }

        public List<UserCustomization> Convert(List<Domain.UserPersonalization> userPersonalization)
        {
            List<Domain.UserCustomization> list = new List<Domain.UserCustomization>();
            foreach (var item in userPersonalization)
            {
                list.Add(Convert(item));
            }
            return list;
        }
    }
}