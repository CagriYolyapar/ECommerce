using Project.MODEL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MAP.Options
{
    public class AppUserDetailMap:BaseMap<AppUserDetail>
    {
        public AppUserDetailMap()
        {
            ToTable("Profiller");
            //HasOptional(x => x.AppUser).WithRequired(x => x.AppUserDetail); => eger üye olmadan işlem yaptıracaksak demek ki uye olmak opsiyonel bir sürectir..Ama detaylara ihtiyacımız olur. Tabii ki bunu kullanıyorsanız diger taraftan da birebir ilişkinin (AppUser tarafından) fluen api mappinginin kaldırılması gerekir.
        }
    }
}
