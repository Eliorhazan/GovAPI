using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovAPI
{
    public partial class MOTModels
    {
        public int Id { get; set; }
        public string sug_degem { get; set; }
        public double? tozeret_cd { get; set; }
        public string tozeret_nm { get; set; }
        public string tozeret_eretz_nm { get; set; }
        public string tozar { get; set; }
        public double? degem_cd { get; set; }
        public string degem_nm { get; set; }
        public string kinuy_mishari { get; set; }
        public string kinuy_mishari_clean { get; set; }
        public double? shnat_yitzur { get; set; }
        public double? kvuzat_agra_cd { get; set; }
        public double? nefah_manoa { get; set; }
        public double? mishkal_kolel { get; set; }
        public double? gova { get; set; }
        public double? hanaa_cd { get; set; }
        public string hanaa_nm { get; set; }
        public double? mazgan_ind { get; set; }
        public double? abs_ind { get; set; }
        public double? kariot_avir_source { get; set; }
        public double? mispar_kariot_avir { get; set; }
        public double? hege_koah_ind { get; set; }
        public double? automatic_ind { get; set; }
        public double? halonot_hashmal_source { get; set; }
        public double? mispar_halonot_hashmal { get; set; }
        public double? halon_bagg_ind { get; set; }
        public double? galgaley_sagsoget_kala_ind { get; set; }
        public double? argaz_ind { get; set; }
        public string merkav { get; set; }
        public string ramat_gimur { get; set; }
        public double? delek_cd { get; set; }
        public string delek_nm { get; set; }
        public double? mispar_dlatot { get; set; }
        public double? koah_sus { get; set; }
        public double? mispar_moshavim { get; set; }
        public double? bakarat_yatzivut_ind { get; set; }
        public double? kosher_grira_im_blamim { get; set; }
        public double? kosher_grira_bli_blamim { get; set; }
        public double? sug_tkina_cd { get; set; }
        public string sug_tkina_nm { get; set; }
        public double? sug_mamir_cd { get; set; }
        public string sug_mamir_nm { get; set; }
        public string technologiat_hanaa_cd { get; set; }
        public string technologiat_hanaa_nm { get; set; }
        public double? kamut_CO2 { get; set; }
        public double? kamut_NOX { get; set; }
        public double? kamut_PM10 { get; set; }
        public double? kamut_HC { get; set; }
        public double? kamut_HC_NOX { get; set; }
        public double? kamut_CO { get; set; }
        public string kamut_CO2_city { get; set; }
        public string kamut_NOX_city { get; set; }
        public string kamut_PM10_city { get; set; }
        public string kamut_HC_city { get; set; }
        public string kamut_CO_city { get; set; }
        public string kamut_CO2_hway { get; set; }
        public string kamut_NOX_hway { get; set; }
        public string kamut_PM10_hway { get; set; }
        public string kamut_HC_hway { get; set; }
        public string kamut_CO_hway { get; set; }
        public double? madad_yarok { get; set; }
        public double? kvutzat_zihum { get; set; }
        public double? bakarat_stiya_menativ_ind { get; set; }
        public string bakarat_stiya_menativ_makor_hatkana { get; set; }
        public double? nitur_merhak_milfanim_ind { get; set; }
        public string nitur_merhak_milfanim_makor_hatkana { get; set; }
        public double? zihuy_beshetah_nistar_ind { get; set; }
        public double? bakarat_shyut_adaptivit_ind { get; set; }
        public double? zihuy_holchey_regel_ind { get; set; }
        public string zihuy_holchey_regel_makor_hatkana { get; set; }
        public double? maarechet_ezer_labalam_ind { get; set; }
        public double? matzlemat_reverse_ind { get; set; }
        public double? hayshaney_lahatz_avir_batzmigim_ind { get; set; }
        public double? hayshaney_hagorot_ind { get; set; }
        public double? nikud_betihut { get; set; }
        public double? ramat_eivzur_betihuty { get; set; }
        public double? teura_automatit_benesiya_kadima_ind { get; set; }
        public double? shlita_automatit_beorot_gvohim_ind { get; set; }
        public string shlita_automatit_beorot_gvohim_makor_hatkana { get; set; }
        public double? zihuy_matzav_hitkarvut_mesukenet_ind { get; set; }
        public double? zihuy_tamrurey_tnua_ind { get; set; }
        public string zihuy_tamrurey_tnua_makor_hatkana { get; set; }
        public string CO2_WLTP { get; set; }
        public string HC_WLTP { get; set; }
        public string PM_WLTP { get; set; }
        public string NOX_WLTP { get; set; }
        public string CO_WLTP { get; set; }
        public string CO2_WLTP_NEDC { get; set; }

        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
    }
}
