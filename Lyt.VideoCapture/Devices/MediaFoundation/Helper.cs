namespace Lyt.VideoCapture.Devices.MediaFoundation;

using global::MediaFoundation;

internal static class Helper
{
    private static readonly Dictionary<Guid, string> GuidToStringDictionary =
        new(64)
        {
            { MFAttributesClsid.MF_MT_MAJOR_TYPE,  "MF_MT_MAJOR_TYPE" },
            { MFAttributesClsid.MF_MT_SUBTYPE, "MF_MT_SUBTYPE" } ,

            { MFMediaType.AI44,   "MFVideoFormat_AI44" } ,  //     FCC('AI44')
            { MFMediaType.ARGB32, "MFVideoFormat_ARGB32" } , //   D3DFMT_A8R8G8B8 
            { MFMediaType.AYUV,   "MFVideoFormat_AYUV" },   //     FCC('AYUV')
            { MFMediaType.DV25,   "MFVideoFormat_DV25" }, //     FCC('dv25')
            { MFMediaType.DV50,   "MFVideoFormat_DV50" }, //     FCC('dv50')
            { MFMediaType.DVH1,   "MFVideoFormat_DVH1" }, //     FCC('dvh1')
            { MFMediaType.DVSD,   "MFVideoFormat_DVSD" }, //     FCC('dvsd')
            { MFMediaType.DVSL,   "MFVideoFormat_DVSL" }, //     FCC('dvsl')
            { MFMediaType.H264,   "MFVideoFormat_H264" }, //     FCC('H264')
            { MFMediaType.I420,   "MFVideoFormat_I420" }, //     FCC('I420')
            { MFMediaType.IYUV,   "MFVideoFormat_IYUV" }, //     FCC('IYUV')
            { MFMediaType.M4S2,   "MFVideoFormat_M4S2" }, //     FCC('M4S2')
            { MFMediaType.MJPG,   "MFVideoFormat_MJPG" },
            { MFMediaType.MP43,   "MFVideoFormat_MP43" }, //     FCC('MP43')
            { MFMediaType.MP4S,   "MFVideoFormat_MP4S" }, //     FCC('MP4S')
            { MFMediaType.MP4V,   "MFVideoFormat_MP4V" }, //     FCC('MP4V')
            { MFMediaType.MPG1,   "MFVideoFormat_MPG1" }, //     FCC('MPG1')
            { MFMediaType.MSS1,   "MFVideoFormat_MSS1" }, //     FCC('MSS1')
            { MFMediaType.MSS2,   "MFVideoFormat_MSS2" }, //     FCC('MSS2')
            { MFMediaType.NV11,   "MFVideoFormat_NV11" }, //     FCC('NV11')
            { MFMediaType.NV12,   "MFVideoFormat_NV12" }, //     FCC('NV12')
            { MFMediaType.P010,   "MFVideoFormat_P010" }, //     FCC('P010')
            { MFMediaType.P016,   "MFVideoFormat_P016" }, //     FCC('P016')
            { MFMediaType.P210,   "MFVideoFormat_P210" }, //     FCC('P210')
            { MFMediaType.P216,   "MFVideoFormat_P216" }, //     FCC('P216')
            { MFMediaType.RGB24,  "MFVideoFormat_RGB24" }, //    D3DFMT_R8G8B8 
            { MFMediaType.RGB32,  "MFVideoFormat_RGB32" }, //    D3DFMT_X8R8G8B8 
            { MFMediaType.RGB555, "MFVideoFormat_RGB555" }, //   D3DFMT_X1R5G5B5 
            { MFMediaType.RGB565, "MFVideoFormat_RGB565" }, //   D3DFMT_R5G6B5 
            { MFMediaType.RGB8,   "MFVideoFormat_RGB8" },
            { MFMediaType.UYVY,   "MFVideoFormat_UYVY" }, //     FCC('UYVY')
            { MFMediaType.v210,   "MFVideoFormat_v210" }, //     FCC('v210')
            { MFMediaType.v410,   "MFVideoFormat_v410" }, //     FCC('v410')
            { MFMediaType.WMV1,   "MFVideoFormat_WMV1" }, //     FCC('WMV1')
            { MFMediaType.WMV2,   "MFVideoFormat_WMV2" }, //     FCC('WMV2')
            { MFMediaType.WMV3,   "MFVideoFormat_WMV3" }, //     FCC('WMV3')
            { MFMediaType.WVC1,   "MFVideoFormat_WVC1" }, //     FCC('WVC1')
            { MFMediaType.Y210,   "MFVideoFormat_Y210" }, //     FCC('Y210')
            { MFMediaType.Y216,   "MFVideoFormat_Y216" }, //     FCC('Y216')
            { MFMediaType.Y410,   "MFVideoFormat_Y410" }, //     FCC('Y410')
            { MFMediaType.Y416,   "MFVideoFormat_Y416" }, //     FCC('Y416')
            { MFMediaType.Y41P,   "MFVideoFormat_Y41P" },
            { MFMediaType.Y41T,   "MFVideoFormat_Y41T" },
            { MFMediaType.YUY2,   "MFVideoFormat_YUY2" }, //     FCC('YUY2')
            { MFMediaType.YV12,   "MFVideoFormat_YV12" }, //     FCC('YV12')
            { MFMediaType.YVYU,   "MFVideoFormat_YVYU" },

            #region Commented out GUID's
            //MF_MT_ALL_SAMPLES_INDEPENDENT, "MF_MT_ALL_SAMPLES_INDEPENDENT",
            //MF_MT_FIXED_SIZE_SAMPLES,                      MF_MT_FIXED_SIZE_SAMPLES,
            //MF_MT_COMPRESSED,                              MF_MT_COMPRESSED,
            //MF_MT_SAMPLE_SIZE,                             MF_MT_SAMPLE_SIZE,
            //MF_MT_WRAPPED_TYPE,                            MF_MT_WRAPPED_TYPE,
            //MF_MT_AUDIO_NUM_CHANNELS,                      MF_MT_AUDIO_NUM_CHANNELS,
            //MF_MT_AUDIO_SAMPLES_PER_SECOND,                MF_MT_AUDIO_SAMPLES_PER_SECOND,
            //MF_MT_AUDIO_FLOAT_SAMPLES_PER_SECOND,          MF_MT_AUDIO_FLOAT_SAMPLES_PER_SECOND,
            //MF_MT_AUDIO_AVG_BYTES_PER_SECOND,              MF_MT_AUDIO_AVG_BYTES_PER_SECOND,
            //MF_MT_AUDIO_BLOCK_ALIGNMENT,                   MF_MT_AUDIO_BLOCK_ALIGNMENT,
            //MF_MT_AUDIO_BITS_PER_SAMPLE,                   MF_MT_AUDIO_BITS_PER_SAMPLE,
            //MF_MT_AUDIO_VALID_BITS_PER_SAMPLE,             MF_MT_AUDIO_VALID_BITS_PER_SAMPLE,
            //MF_MT_AUDIO_SAMPLES_PER_BLOCK,                 MF_MT_AUDIO_SAMPLES_PER_BLOCK,
            //MF_MT_AUDIO_CHANNEL_MASK,                      MF_MT_AUDIO_CHANNEL_MASK,
            //MF_MT_AUDIO_FOLDDOWN_MATRIX,                   MF_MT_AUDIO_FOLDDOWN_MATRIX,
            //MF_MT_AUDIO_WMADRC_PEAKREF,                    MF_MT_AUDIO_WMADRC_PEAKREF,
            //MF_MT_AUDIO_WMADRC_PEAKTARGET,                 MF_MT_AUDIO_WMADRC_PEAKTARGET,
            //MF_MT_AUDIO_WMADRC_AVGREF,                     MF_MT_AUDIO_WMADRC_AVGREF,
            //MF_MT_AUDIO_WMADRC_AVGTARGET,                  MF_MT_AUDIO_WMADRC_AVGTARGET,
            //MF_MT_AUDIO_PREFER_WAVEFORMATEX,               MF_MT_AUDIO_PREFER_WAVEFORMATEX,
            //MF_MT_AAC_PAYLOAD_TYPE,                        MF_MT_AAC_PAYLOAD_TYPE,
            //MF_MT_AAC_AUDIO_PROFILE_LEVEL_INDICATION,      MF_MT_AAC_AUDIO_PROFILE_LEVEL_INDICATION,
            //MF_MT_FRAME_SIZE,                              MF_MT_FRAME_SIZE,
            //MF_MT_FRAME_RATE,                              MF_MT_FRAME_RATE,
            //MF_MT_FRAME_RATE_RANGE_MAX,                    MF_MT_FRAME_RATE_RANGE_MAX,
            //MF_MT_FRAME_RATE_RANGE_MIN,                    MF_MT_FRAME_RATE_RANGE_MIN,
            //MF_MT_PIXEL_ASPECT_RATIO,                      MF_MT_PIXEL_ASPECT_RATIO,
            //MF_MT_DRM_FLAGS,                               MF_MT_DRM_FLAGS,
            //MF_MT_PAD_CONTROL_FLAGS,                       MF_MT_PAD_CONTROL_FLAGS,
            //MF_MT_SOURCE_CONTENT_HINT,                     MF_MT_SOURCE_CONTENT_HINT,
            //MF_MT_VIDEO_CHROMA_SITING,                     MF_MT_VIDEO_CHROMA_SITING,
            //MF_MT_INTERLACE_MODE,                          MF_MT_INTERLACE_MODE,
            //MF_MT_TRANSFER_FUNCTION,                       MF_MT_TRANSFER_FUNCTION,
            //MF_MT_VIDEO_PRIMARIES,                         MF_MT_VIDEO_PRIMARIES,
            //MF_MT_CUSTOM_VIDEO_PRIMARIES,                  MF_MT_CUSTOM_VIDEO_PRIMARIES,
            //MF_MT_YUV_MATRIX,                              MF_MT_YUV_MATRIX,
            //MF_MT_VIDEO_LIGHTING,                          MF_MT_VIDEO_LIGHTING,
            //MF_MT_VIDEO_NOMINAL_RANGE,                     MF_MT_VIDEO_NOMINAL_RANGE,
            //MF_MT_GEOMETRIC_APERTURE,                      MF_MT_GEOMETRIC_APERTURE,
            //MF_MT_MINIMUM_DISPLAY_APERTURE,                MF_MT_MINIMUM_DISPLAY_APERTURE,
            //MF_MT_PAN_SCAN_APERTURE,                       MF_MT_PAN_SCAN_APERTURE,
            //MF_MT_PAN_SCAN_ENABLED,                        MF_MT_PAN_SCAN_ENABLED,
            //MF_MT_AVG_BITRATE,                             MF_MT_AVG_BITRATE,
            //MF_MT_AVG_BIT_ERROR_RATE,                      MF_MT_AVG_BIT_ERROR_RATE,
            //MF_MT_MAX_KEYFRAME_SPACING,                    MF_MT_MAX_KEYFRAME_SPACING,
            //MF_MT_DEFAULT_STRIDE,                          MF_MT_DEFAULT_STRIDE,
            //MF_MT_PALETTE,                                 MF_MT_PALETTE,
            //MF_MT_USER_DATA,                               MF_MT_USER_DATA,
            //MF_MT_AM_FORMAT_TYPE,                          MF_MT_AM_FORMAT_TYPE,
            //MF_MT_MPEG_START_TIME_CODE,                    MF_MT_MPEG_START_TIME_CODE,
            //MF_MT_MPEG2_PROFILE,                           MF_MT_MPEG2_PROFILE,
            //MF_MT_MPEG2_LEVEL,                             MF_MT_MPEG2_LEVEL,
            //MF_MT_MPEG2_FLAGS,                             MF_MT_MPEG2_FLAGS,
            //MF_MT_MPEG_SEQUENCE_HEADER,                    MF_MT_MPEG_SEQUENCE_HEADER,
            //MF_MT_DV_AAUX_SRC_PACK_0,                      MF_MT_DV_AAUX_SRC_PACK_0,
            //MF_MT_DV_AAUX_CTRL_PACK_0,                     MF_MT_DV_AAUX_CTRL_PACK_0,
            //MF_MT_DV_AAUX_SRC_PACK_1,                      MF_MT_DV_AAUX_SRC_PACK_1,
            //MF_MT_DV_AAUX_CTRL_PACK_1,                     MF_MT_DV_AAUX_CTRL_PACK_1,
            //MF_MT_DV_VAUX_SRC_PACK,                        MF_MT_DV_VAUX_SRC_PACK,
            //MF_MT_DV_VAUX_CTRL_PACK,                       MF_MT_DV_VAUX_CTRL_PACK,
            //MF_MT_ARBITRARY_HEADER,                        MF_MT_ARBITRARY_HEADER,
            //MF_MT_ARBITRARY_FORMAT,                        MF_MT_ARBITRARY_FORMAT,
            //MF_MT_IMAGE_LOSS_TOLERANT,                     MF_MT_IMAGE_LOSS_TOLERANT,
            //MF_MT_MPEG4_SAMPLE_DESCRIPTION,                MF_MT_MPEG4_SAMPLE_DESCRIPTION,
            //MF_MT_MPEG4_CURRENT_SAMPLE_ENTRY,              MF_MT_MPEG4_CURRENT_SAMPLE_ENTRY,
            //MF_MT_ORIGINAL_4CC,                            MF_MT_ORIGINAL_4CC,
            //MF_MT_ORIGINAL_WAVE_FORMAT_TAG,                MF_MT_ORIGINAL_WAVE_FORMAT_TAG,
            //MFMediaType_Audio,                             MFMediaType_Audio,
            //MFMediaType_Video,                             MFMediaType_Video,
            //MFMediaType_Protected,                         MFMediaType_Protected,
            //MFMediaType_SAMI,                              MFMediaType_SAMI,
            //MFMediaType_Script,                            MFMediaType_Script,
            //MFMediaType_Image,                             MFMediaType_Image,
            //MFMediaType_HTML,                              MFMediaType_HTML,
            //MFMediaType_Binary,                            MFMediaType_Binary,
            //MFMediaType_FileTransfer,                      MFMediaType_FileTransfer,
            //MFAudioFormat_PCM, //              WAVE_FORMAT_MFAudioFormat_PCM, //              WAVE_FORPCM 
            //MFAudioFormat_Float, //            WAVE_FORMAT_MFAudioFormat_Float, //            WAVE_FORIEEE_FLOAT 
            //MFAudioFormat_DTS, //              WAVE_FORMAT_MFAudioFormat_DTS, //              WAVE_FORDTS 
            //MFAudioFormat_Dolby_AC3_SPDIF, //  WAVE_FORMAT_MFAudioFormat_Dolby_AC3_SPDIF, //  WAVE_FORDOLBY_AC3_SPDIF 
            //MFAudioFormat_DRM, //              WAVE_FORMAT_MFAudioFormat_DRM, //              WAVE_FORDRM 
            //MFAudioFormat_WMAudioV8, //        WAVE_FORMAT_MFAudioFormat_WMAudioV8, //        WAVE_FORWMAUDIO2 
            //MFAudioFormat_WMAudioV9, //        WAVE_FORMAT_MFAudioFormat_WMAudioV9, //        WAVE_FORWMAUDIO3 
            //MFAudioFormat_WMAudio_Lossless, // WAVE_FORMAT_MFAudioFormat_WMAudio_Lossless, // WAVE_FORWMAUDIO_LOSSLESS 
            //MFAudioFormat_WMASPDIF, //         WAVE_FORMAT_MFAudioFormat_WMASPDIF, //         WAVE_FORWMASPDIF 
            //MFAudioFormat_MSP1, //             WAVE_FORMAT_MFAudioFormat_MSP1, //             WAVE_FORWMAVOICE9 
            //MFAudioFormat_MP3, //              WAVE_FORMAT_MFAudioFormat_MP3, //              WAVE_FORMPEGLAYER3 
            //MFAudioFormat_MPEG, //             WAVE_FORMAT_MFAudioFormat_MPEG, //             WAVE_FORMPEG 
            //MFAudioFormat_AAC, //              WAVE_FORMAT_MFAudioFormat_AAC, //              WAVE_FORMPEG_HEAAC 
            //MFAudioFormat_ADTS, //             WAVE_FORMAT_MFAudioFormat_ADTS, //             WAVE_FORMPEG_ADTS_AAC 
        #endregion 
        };

    public static Guid FromName(this string name)
        => (from kvp in Helper.GuidToStringDictionary where (kvp.Value == name) select kvp.Key).FirstOrDefault();

    public static string ToName(this Guid guid)
        => Helper.GuidToStringDictionary.TryGetValue(guid, out string? value) ? value : string.Empty;

    // Return the GUID associated with a MF Interface, for example the IMFMediaSource interface, etc. 
    public static Guid GetGuid<T>()
    {
        var customAttributes = typeof(T).GetCustomAttributes(inherit: false);
        foreach (var attribute in customAttributes)
        {
            if (attribute is GuidAttribute guidAttribute)
            {
                return new Guid(guidAttribute.Value);
            }
        }

        return Guid.Empty;
    }

    public static int High32(this ulong value) => (int)((value & 0xFFFF_FFFF_0000_0000) >> 32);

    public static int Low32(this ulong value) => (int)(value & 0x0000_FFFF_FFFF);

}
