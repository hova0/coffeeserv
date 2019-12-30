﻿using System;
using System.Collections.Generic;
using System.Text;

namespace coffeeserv
{
    public enum EPD_Commands
    {
        PANEL_SETTING                               = 0x00,
        POWER_SETTING                               = 0x01,
        POWER_OFF                                   = 0x02,
        POWER_OFF_SEQUENCE_SETTING                  = 0x03,
        POWER_ON                                    = 0x04,
        POWER_ON_MEASURE                            = 0x05,
        BOOSTER_SOFT_START                          = 0x06,
        DEEP_SLEEP                                  = 0x07,
        DATA_START_TRANSMISSION_1                   = 0x10,
        DATA_STOP                                   = 0x11,
        DISPLAY_REFRESH                             = 0x12,
        DATA_START_TRANSMISSION_2                   = 0x13,
        PARTIAL_DATA_START_TRANSMISSION_1           = 0x14,
        PARTIAL_DATA_START_TRANSMISSION_2           = 0x15,
        PARTIAL_DISPLAY_REFRESH                     = 0x16,
        LUT_FOR_VCOM                                = 0x20,
        LUT_WHITE_TO_WHITE                          = 0x21,
        LUT_BLACK_TO_WHITE                          = 0x22,
        LUT_WHITE_TO_BLACK                          = 0x23,
        LUT_BLACK_TO_BLACK                          = 0x24,
        PLL_CONTROL                                 = 0x30,
        TEMPERATURE_SENSOR_COMMAND                  = 0x40,
        TEMPERATURE_SENSOR_CALIBRATION              = 0x41,
        TEMPERATURE_SENSOR_WRITE                    = 0x42,
        TEMPERATURE_SENSOR_READ                     = 0x43,
        VCOM_AND_DATA_INTERVAL_SETTING              = 0x50,
        LOW_POWER_DETECTION                         = 0x51,
        TCON_SETTING                                = 0x60,
        TCON_RESOLUTION                             = 0x61,
        SOURCE_AND_GATE_START_SETTING               = 0x62,
        GET_STATUS                                  = 0x71,
        AUTO_MEASURE_VCOM                           = 0x80,
        VCOM_VALUE                                  = 0x81,
        VCM_DC_SETTING_REGISTER                     = 0x82,
        PROGRAM_MODE                                = 0xA0,
        ACTIVE_PROGRAM                              = 0xA1,
        READ_OTP_DATA                               = 0xA2,
        POWER_OPTIMIZATION                          = 0xF8
    }
}