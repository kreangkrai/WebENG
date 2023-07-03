using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebENG.Models;

namespace WebENG.Interface
{
    interface ISkill
    {
        List<EngSkillModel> GetSkills();
        int GetLastSkillID();
        string CreateSkill(EngSkillModel skill);
        string EditSkill(EngSkillModel skill);
        string DeleteSkill(EngSkillModel skill);
    }
}
