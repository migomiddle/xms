using System;
using System.Collections.Generic;

namespace Xms.UserPersonalization
{
    /// <summary>
    /// 用户个性化服务
    /// </summary>
    public class UserPersonalizationService : IUserPersonalizationService
    {
        private readonly Data.IUserPersonalizationRepository _userPersonalizationRepository;

        public UserPersonalizationService(Data.IUserPersonalizationRepository userPersonalizationRepository)
        {
            _userPersonalizationRepository = userPersonalizationRepository;
        }

        public List<Domain.UserPersonalization> Get(Guid ownerId)
        {
            return _userPersonalizationRepository.Get(ownerId);
        }

        public Domain.UserPersonalization GetByName(Guid ownerId, string name)
        {
            return _userPersonalizationRepository.GetByName(ownerId, name);
        }

        public bool Set(Domain.UserPersonalization userPersonalization)
        {
            return _userPersonalizationRepository.Set(userPersonalization);
        }

        public bool Delete(Guid ownerId)
        {
            return _userPersonalizationRepository.Delete(ownerId);
        }

        public bool Delete(Guid ownerId, string name)
        {
            return _userPersonalizationRepository.Delete(ownerId, name);
        }

        public bool DeleteById(Guid id)
        {
            return _userPersonalizationRepository.DeleteById(id);
        }
    }
}