﻿// ----------------------------------------------------------------------
// <copyright file="NDS.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>28/04/2012 14:26:27</date>
// 
// Modified (DSi): Metlob, 13.10.2017
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using Ekona;
using Ekona.Images;
using Ekona.Helper;

namespace Tinke.Nitro
{
    public static class NDS
    {
        public static Estructuras.ROMHeader LeerCabecera(string file)
        {
            Estructuras.ROMHeader nds = new Estructuras.ROMHeader();

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            Console.WriteLine("<b>" +
                Tools.Helper.GetTranslation("Messages", "S03") + "</b> "
                + new FileInfo(file).Name);

            Rellenar_MakerCodes();
            Rellenar_UnitCodes();

            nds.gameTitle = br.ReadChars(12);
            nds.gameCode = br.ReadChars(4);
            nds.makerCode = br.ReadChars(2);
            nds.unitCode = br.ReadByte();
            nds.encryptionSeed = br.ReadByte();
            nds.tamaño = (UInt32)Math.Pow(2, 17 + br.ReadByte());
            nds.reserved = br.ReadBytes(7);
            nds.twlInternalFlags = br.ReadByte();
            nds.permitsFlags = br.ReadByte();
            nds.ROMversion = br.ReadByte();
            nds.internalFlags = br.ReadByte();
            nds.ARM9romOffset = br.ReadUInt32();
            nds.ARM9entryAddress = br.ReadUInt32();
            nds.ARM9ramAddress = br.ReadUInt32();
            nds.ARM9size = br.ReadUInt32();
            nds.ARM7romOffset = br.ReadUInt32();
            nds.ARM7entryAddress = br.ReadUInt32();
            nds.ARM7ramAddress = br.ReadUInt32();
            nds.ARM7size = br.ReadUInt32();
            nds.fileNameTableOffset = br.ReadUInt32();
            nds.fileNameTableSize = br.ReadUInt32();
            nds.FAToffset = br.ReadUInt32();
            nds.FATsize = br.ReadUInt32();
            nds.ARM9overlayOffset = br.ReadUInt32();
            nds.ARM9overlaySize = br.ReadUInt32();
            nds.ARM7overlayOffset = br.ReadUInt32();
            nds.ARM7overlaySize = br.ReadUInt32();
            nds.flagsRead = br.ReadUInt32();
            nds.flagsInit = br.ReadUInt32();
            nds.bannerOffset = br.ReadUInt32();
            nds.secureCRC16 = br.ReadUInt16();
            nds.ROMtimeout = br.ReadUInt16();
            nds.ARM9autoload = br.ReadUInt32();
            nds.ARM7autoload = br.ReadUInt32();
            nds.secureDisable = br.ReadUInt64();
            nds.ROMsize = br.ReadUInt32();
            nds.headerSize = br.ReadUInt32();
            nds.reserved2 = br.ReadBytes(56);
            br.BaseStream.Seek(156, SeekOrigin.Current); //nds.logo = br.ReadBytes(156); Logo de Nintendo utilizado para comprobaciones
            nds.logoCRC16 = br.ReadUInt16();
            nds.headerCRC16 = br.ReadUInt16();
            nds.debug_romOffset = br.ReadUInt32();
            nds.debug_size = br.ReadUInt32();
            nds.debug_ramAddress = br.ReadUInt32();
            nds.reserved3 = br.ReadUInt32();
            nds.reserved4 = br.ReadBytes(0x10);

            // DSi extended stuff below
            if (nds.headerSize > 0x200 && (nds.unitCode & 2) > 0)
            {
                nds.global_mbk_setting = new byte[5][];
                for (int i = 0; i < 5; i++) nds.global_mbk_setting[i] = br.ReadBytes(4);
                nds.arm9_mbk_setting = new uint[3];
                for (int i = 0; i < 3; i++) nds.arm9_mbk_setting[i] = br.ReadUInt32();
                nds.arm7_mbk_setting = new uint[3];
                for (int i = 0; i < 3; i++) nds.arm7_mbk_setting[i] = br.ReadUInt32();
                nds.mbk9_wramcnt_setting = br.ReadUInt32();

                nds.region_flags = br.ReadUInt32();
                nds.access_control = br.ReadUInt32();
                nds.scfg_ext_mask = br.ReadUInt32();
                nds.appflags = br.ReadBytes(4);

                nds.dsi9_rom_offset = br.ReadUInt32();
                nds.offset_0x1C4 = br.ReadUInt32();
                nds.dsi9_ram_address = br.ReadUInt32();
                nds.dsi9_size = br.ReadUInt32();
                nds.dsi7_rom_offset = br.ReadUInt32();
                nds.offset_0x1D4 = br.ReadUInt32();
                nds.dsi7_ram_address = br.ReadUInt32();
                nds.dsi7_size = br.ReadUInt32();

                nds.digest_ntr_start = br.ReadUInt32();
                nds.digest_ntr_size = br.ReadUInt32();
                nds.digest_twl_start = br.ReadUInt32();
                nds.digest_twl_size = br.ReadUInt32();

                nds.sector_hashtable_start = br.ReadUInt32();
                nds.sector_hashtable_size = br.ReadUInt32();
                nds.block_hashtable_start = br.ReadUInt32();
                nds.block_hashtable_size = br.ReadUInt32();

                nds.digest_sector_size = br.ReadUInt32();
                nds.digest_block_sectorcount = br.ReadUInt32();
                nds.banner_size = br.ReadUInt32();
                nds.offset_0x20C = br.ReadUInt32();

                nds.total_rom_size = br.ReadUInt32();
                nds.offset_0x214 = br.ReadUInt32();
                nds.offset_0x218 = br.ReadUInt32();
                nds.offset_0x21C = br.ReadUInt32();

                nds.modcrypt1_start = br.ReadUInt32();
                nds.modcrypt1_size = br.ReadUInt32();
                nds.modcrypt2_start = br.ReadUInt32();
                nds.modcrypt2_size = br.ReadUInt32();

                nds.tid_low = br.ReadUInt32();
                nds.tid_high = br.ReadUInt32();
                nds.public_sav_size = br.ReadUInt32();
                nds.private_sav_size = br.ReadUInt32();

                nds.reserved5 = br.ReadBytes(0xB0);
                nds.age_ratings = br.ReadBytes(0x10);
                nds.hmac_arm9 = br.ReadBytes(20);
                nds.hmac_arm7 = br.ReadBytes(20);
                nds.hmac_digest_master = br.ReadBytes(20);
                nds.hmac_icon_title = br.ReadBytes(20);
                nds.hmac_arm9i = br.ReadBytes(20);
                nds.hmac_arm7i = br.ReadBytes(20);
                nds.reserved6 = br.ReadBytes(40);
                nds.hmac_arm9_no_secure = br.ReadBytes(20);
                nds.reserved7 = br.ReadBytes(0xA4C);
                nds.debug_args = br.ReadBytes(0x180);
                nds.rsa_signature = br.ReadBytes(0x80);

                //nds.trimmedRom = br.BaseStream.Length == nds.total_rom_size;
                nds.doublePadding = (nds.ARM9size % 0x400) < 0x200 && nds.ARM7romOffset % 0x400 == 0 && nds.ARM9overlayOffset % 0x400 == 0;
                nds.doublePadding |= (nds.ARM7size % 0x400) < 0x200 && nds.fileNameTableOffset % 0x400 == 0 && nds.ARM7overlayOffset % 0x400 == 0;
                nds.doublePadding |= (nds.fileNameTableSize % 0x400) < 0x200 && nds.FAToffset % 0x400 == 0;
                nds.doublePadding |= (nds.FATsize % 0x400) < 0x200 && nds.bannerOffset % 0x400 == 0;
            }
            if (nds.total_rom_size != 0)
                nds.trimmedRom = (nds.total_rom_size - br.BaseStream.Length >= 0);
            else
                nds.trimmedRom = br.BaseStream.Length != nds.tamaño;

            // Calc CRCs
            uint gameCode = BitConverter.ToUInt32(Encoding.ASCII.GetBytes(nds.gameCode), 0);
            br.BaseStream.Position = 0x4000;
            nds.secureCRC = (SecureArea.CalcCRC(br.ReadBytes(0x4000), gameCode) == nds.secureCRC16) ? true : false;
            br.BaseStream.Position = 0xC0;
            nds.logoCRC = (CRC16.Calculate(br.ReadBytes(156)) == nds.logoCRC16) ? true : false;
            br.BaseStream.Position = 0x0;
            nds.headerCRC = (CRC16.Calculate(br.ReadBytes(0x15E)) == nds.headerCRC16) ? true : false;

            br.Close();

            Console.WriteLine("<b>" +
                Tools.Helper.GetTranslation("Messages", "S04")
                + "</b><br>" + new String(nds.gameTitle).Replace("\0", "") +
                " (" + new String(nds.gameCode).Replace("\0", "") + ')');

            return nds;
        }
        public static void EscribirCabecera(string salida, Estructuras.ROMHeader cabecera, string romFile)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(salida));
            BinaryReader br = new BinaryReader(File.OpenRead(romFile));
            br.BaseStream.Position = 0xC0;
            Console.Write(Tools.Helper.GetTranslation("Messages", "S0A"));

            bw.Write(cabecera.gameTitle);
            bw.Write(cabecera.gameCode);
            bw.Write(cabecera.makerCode);
            bw.Write(cabecera.unitCode);
            bw.Write(cabecera.encryptionSeed);
            bw.Write((byte)(Math.Log(cabecera.tamaño, 2) - 17));
            bw.Write(cabecera.reserved);
            bw.Write(cabecera.twlInternalFlags);
            bw.Write(cabecera.permitsFlags);
            bw.Write(cabecera.ROMversion);
            bw.Write(cabecera.internalFlags);
            bw.Write(cabecera.ARM9romOffset);
            bw.Write(cabecera.ARM9entryAddress);
            bw.Write(cabecera.ARM9ramAddress);
            bw.Write(cabecera.ARM9size);
            bw.Write(cabecera.ARM7romOffset);
            bw.Write(cabecera.ARM7entryAddress);
            bw.Write(cabecera.ARM7ramAddress);
            bw.Write(cabecera.ARM7size);
            bw.Write(cabecera.fileNameTableOffset);
            bw.Write(cabecera.fileNameTableSize);
            bw.Write(cabecera.FAToffset);
            bw.Write(cabecera.FATsize);
            bw.Write(cabecera.ARM9overlayOffset);
            bw.Write(cabecera.ARM9overlaySize);
            bw.Write(cabecera.ARM7overlayOffset);
            bw.Write(cabecera.ARM7overlaySize);
            bw.Write(cabecera.flagsRead);
            bw.Write(cabecera.flagsInit);
            bw.Write(cabecera.bannerOffset);
            bw.Write(cabecera.secureCRC16);
            bw.Write(cabecera.ROMtimeout);
            bw.Write(cabecera.ARM9autoload);
            bw.Write(cabecera.ARM7autoload);
            bw.Write(cabecera.secureDisable);
            bw.Write(cabecera.ROMsize);
            bw.Write(cabecera.headerSize);
            bw.Write(cabecera.reserved2);
            bw.Write(br.ReadBytes(0x9C));
            bw.Write(cabecera.logoCRC16);
            bw.Write(cabecera.headerCRC16);
            bw.Write(cabecera.debug_romOffset);
            bw.Write(cabecera.debug_size);
            bw.Write(cabecera.debug_ramAddress);
            bw.Write(cabecera.reserved3);
            bw.Write(cabecera.reserved4);

            // Write DSi rom info
            if (cabecera.headerSize == 0x4000 && (cabecera.unitCode & 2) > 0)
            {
                for (int i = 0; i < 5; i++) bw.Write(cabecera.global_mbk_setting[i]);
                for (int i = 0; i < 3; i++) bw.Write(cabecera.arm9_mbk_setting[i]);
                for (int i = 0; i < 3; i++) bw.Write(cabecera.arm7_mbk_setting[i]);
                bw.Write(cabecera.mbk9_wramcnt_setting);

                bw.Write(cabecera.region_flags);
                bw.Write(cabecera.access_control);
                bw.Write(cabecera.scfg_ext_mask);
                bw.Write(cabecera.appflags);

                bw.Write(cabecera.dsi9_rom_offset);
                bw.Write(cabecera.offset_0x1C4);
                bw.Write(cabecera.dsi9_ram_address);
                bw.Write(cabecera.dsi9_size);
                bw.Write(cabecera.dsi7_rom_offset);
                bw.Write(cabecera.offset_0x1D4);
                bw.Write(cabecera.dsi7_ram_address);
                bw.Write(cabecera.dsi7_size);

                bw.Write(cabecera.digest_ntr_start);
                bw.Write(cabecera.digest_ntr_size);
                bw.Write(cabecera.digest_twl_start);
                bw.Write(cabecera.digest_twl_size);

                bw.Write(cabecera.sector_hashtable_start);
                bw.Write(cabecera.sector_hashtable_size);
                bw.Write(cabecera.block_hashtable_start);
                bw.Write(cabecera.block_hashtable_size);

                bw.Write(cabecera.digest_sector_size);
                bw.Write(cabecera.digest_block_sectorcount);
                bw.Write(cabecera.banner_size);
                bw.Write(cabecera.offset_0x20C);

                bw.Write(cabecera.total_rom_size);
                bw.Write(cabecera.offset_0x214);
                bw.Write(cabecera.offset_0x218);
                bw.Write(cabecera.offset_0x21C);

                bw.Write(cabecera.modcrypt1_start);
                bw.Write(cabecera.modcrypt1_size);
                bw.Write(cabecera.modcrypt2_start);
                bw.Write(cabecera.modcrypt2_size);

                bw.Write(cabecera.tid_low);
                bw.Write(cabecera.tid_high);
                bw.Write(cabecera.public_sav_size);
                bw.Write(cabecera.private_sav_size);

                bw.Write(cabecera.reserved5);
                bw.Write(cabecera.age_ratings);
                bw.Write(cabecera.hmac_arm9);
                bw.Write(cabecera.hmac_arm7);
                bw.Write(cabecera.hmac_digest_master);
                bw.Write(cabecera.hmac_icon_title);
                bw.Write(cabecera.hmac_arm9i);
                bw.Write(cabecera.hmac_arm7i);
                bw.Write(cabecera.reserved6);
                bw.Write(cabecera.hmac_arm9_no_secure);
                bw.Write(cabecera.reserved7);
                bw.Write(cabecera.debug_args);
                bw.Write(cabecera.rsa_signature);
            }

            int relleno = (int)(cabecera.headerSize - bw.BaseStream.Length);
            br.BaseStream.Position = bw.BaseStream.Position;
            for (int i = 0; i < relleno; i++)
                bw.Write(br.ReadByte());

            bw.Flush();
            bw.Close();
            br.Close();

            Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S09"), new FileInfo(salida).Length);
        }
        public static void EscribirCabecera(string salida, Estructuras.ROMHeader cabecera, byte[] nintendoLogo)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(salida));
            Console.Write(Tools.Helper.GetTranslation("Messages", "S0A"));

            bw.Write(cabecera.gameTitle);
            bw.Write(cabecera.gameCode);
            bw.Write(cabecera.makerCode);
            bw.Write(cabecera.unitCode);
            bw.Write(cabecera.encryptionSeed);
            bw.Write((byte)(Math.Log(cabecera.tamaño, 2) - 17));
            bw.Write(cabecera.reserved);
            bw.Write(cabecera.twlInternalFlags);
            bw.Write(cabecera.permitsFlags);
            bw.Write(cabecera.ROMversion);
            bw.Write(cabecera.internalFlags);
            bw.Write(cabecera.ARM9romOffset);
            bw.Write(cabecera.ARM9entryAddress);
            bw.Write(cabecera.ARM9ramAddress);
            bw.Write(cabecera.ARM9size);
            bw.Write(cabecera.ARM7romOffset);
            bw.Write(cabecera.ARM7entryAddress);
            bw.Write(cabecera.ARM7ramAddress);
            bw.Write(cabecera.ARM7size);
            bw.Write(cabecera.fileNameTableOffset);
            bw.Write(cabecera.fileNameTableSize);
            bw.Write(cabecera.FAToffset);
            bw.Write(cabecera.FATsize);
            bw.Write(cabecera.ARM9overlayOffset);
            bw.Write(cabecera.ARM9overlaySize);
            bw.Write(cabecera.ARM7overlayOffset);
            bw.Write(cabecera.ARM7overlaySize);
            bw.Write(cabecera.flagsRead);
            bw.Write(cabecera.flagsInit);
            bw.Write(cabecera.bannerOffset);
            bw.Write(cabecera.secureCRC16);
            bw.Write(cabecera.ROMtimeout);
            bw.Write(cabecera.ARM9autoload);
            bw.Write(cabecera.ARM7autoload);
            bw.Write(cabecera.secureDisable);
            bw.Write(cabecera.ROMsize);
            bw.Write(cabecera.headerSize);
            bw.Write(cabecera.reserved2);
            bw.Write(nintendoLogo);
            bw.Write(cabecera.logoCRC16);
            bw.Write(cabecera.headerCRC16);
            bw.Write(cabecera.debug_romOffset);
            bw.Write(cabecera.debug_size);
            bw.Write(cabecera.debug_ramAddress);
            bw.Write(cabecera.reserved3);
            bw.Write(cabecera.reserved4);

            // Write DSi rom info
            if (cabecera.headerSize == 0x4000 && (cabecera.unitCode & 2) > 0)
            {
                for (int i = 0; i < 5; i++) bw.Write(cabecera.global_mbk_setting[i]);
                for (int i = 0; i < 3; i++) bw.Write(cabecera.arm9_mbk_setting[i]);
                for (int i = 0; i < 3; i++) bw.Write(cabecera.arm7_mbk_setting[i]);
                bw.Write(cabecera.mbk9_wramcnt_setting);

                bw.Write(cabecera.region_flags);
                bw.Write(cabecera.access_control);
                bw.Write(cabecera.scfg_ext_mask);
                bw.Write(cabecera.appflags);

                bw.Write(cabecera.dsi9_rom_offset);
                bw.Write(cabecera.offset_0x1C4);
                bw.Write(cabecera.dsi9_ram_address);
                bw.Write(cabecera.dsi9_size);
                bw.Write(cabecera.dsi7_rom_offset);
                bw.Write(cabecera.offset_0x1D4);
                bw.Write(cabecera.dsi7_ram_address);
                bw.Write(cabecera.dsi7_size);

                bw.Write(cabecera.digest_ntr_start);
                bw.Write(cabecera.digest_ntr_size);
                bw.Write(cabecera.digest_twl_start);
                bw.Write(cabecera.digest_twl_size);

                bw.Write(cabecera.sector_hashtable_start);
                bw.Write(cabecera.sector_hashtable_size);
                bw.Write(cabecera.block_hashtable_start);
                bw.Write(cabecera.block_hashtable_size);

                bw.Write(cabecera.digest_sector_size);
                bw.Write(cabecera.digest_block_sectorcount);
                bw.Write(cabecera.banner_size);
                bw.Write(cabecera.offset_0x20C);

                bw.Write(cabecera.total_rom_size);
                bw.Write(cabecera.offset_0x214);
                bw.Write(cabecera.offset_0x218);
                bw.Write(cabecera.offset_0x21C);

                bw.Write(cabecera.modcrypt1_start);
                bw.Write(cabecera.modcrypt1_size);
                bw.Write(cabecera.modcrypt2_start);
                bw.Write(cabecera.modcrypt2_size);

                bw.Write(cabecera.tid_low);
                bw.Write(cabecera.tid_high);
                bw.Write(cabecera.public_sav_size);
                bw.Write(cabecera.private_sav_size);

                bw.Write(cabecera.reserved5);
                bw.Write(cabecera.age_ratings);
                bw.Write(cabecera.hmac_arm9);
                bw.Write(cabecera.hmac_arm7);
                bw.Write(cabecera.hmac_digest_master);
                bw.Write(cabecera.hmac_icon_title);
                bw.Write(cabecera.hmac_arm9i);
                bw.Write(cabecera.hmac_arm7i);
                bw.Write(cabecera.reserved6);
                bw.Write(cabecera.hmac_arm9_no_secure);
                bw.Write(cabecera.reserved7);
                bw.Write(cabecera.debug_args);
                bw.Write(cabecera.rsa_signature);
            }

            int relleno = (int)(cabecera.headerSize - bw.BaseStream.Length);
            for (int i = 0; i < relleno; i++)
                bw.Write((byte)0x00);

            bw.Flush();
            bw.Close();

            Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S09"), new FileInfo(salida).Length);
        }

        public static Estructuras.Banner LeerBanner(string file, UInt32 offset, UInt32 size)
        {
            Estructuras.Banner bn = new Estructuras.Banner();
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            br.BaseStream.Position = offset;

            bn.version = br.ReadUInt16();
            bn.CRC16 = br.ReadUInt16();
            bn.CRC162 = br.ReadUInt16();
            bn.CRC163 = br.ReadUInt16();
            bn.CRC16i = br.ReadUInt16();
            bn.reserved = br.ReadBytes(0x16);
            bn.tileData = br.ReadBytes(0x200);
            bn.palette = br.ReadBytes(0x20);
            bn.japaneseTitle = TitleToString(br.ReadBytes(0x100));
            bn.englishTitle = TitleToString(br.ReadBytes(0x100));
            bn.frenchTitle = TitleToString(br.ReadBytes(0x100));
            bn.germanTitle = TitleToString(br.ReadBytes(0x100));
            bn.italianTitle = TitleToString(br.ReadBytes(0x100));
            bn.spanishTitle = TitleToString(br.ReadBytes(0x100));

            // Version 2-3
            //byte v = (byte)(bn.version & 0xFF);
            if (bn.version >= 2) bn.chineseTitle = TitleToString(br.ReadBytes(0x100));
            if (bn.version >= 3) bn.koreanTitle = TitleToString(br.ReadBytes(0x100));
            if (bn.version == 2) bn.padding2 = br.ReadBytes(0xC0);
            if (bn.version == 3) bn.padding3 = br.ReadBytes(0x1C0);
            // DSi Enhanced
            if (bn.version >> 8 == 1)
            {
                bn.reservedDsi = br.ReadBytes(0x800);
                if (size == 0 || size == 0xFFFFFFFF || size > 0x23C0) size = 0x23C0;
                bn.aniIconData = br.ReadBytes((int)(offset + size - br.BaseStream.Position));
            }

            br.BaseStream.Position = offset + 0x20;
            bn.checkCRC = (CRC16.Calculate(br.ReadBytes(0x820)) == bn.CRC16);
            br.Close();

            Console.WriteLine(bn.englishTitle.Replace("\0", ""));

            return bn;
        }
        public static uint EscribirBanner(string salida, Estructuras.Banner banner)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(salida, FileMode.Create));
            Console.Write("Banner...");

            bw.Write(banner.version);
            bw.Write(banner.CRC16);
            bw.Write(banner.CRC162);
            bw.Write(banner.CRC163);
            bw.Write(banner.CRC16i);
            bw.Write(banner.reserved);
            bw.Write(banner.tileData);
            bw.Write(banner.palette);
            bw.Write(StringToTitle(banner.japaneseTitle));
            bw.Write(StringToTitle(banner.englishTitle));
            bw.Write(StringToTitle(banner.frenchTitle));
            bw.Write(StringToTitle(banner.germanTitle));
            bw.Write(StringToTitle(banner.italianTitle));
            bw.Write(StringToTitle(banner.spanishTitle));

            // Version 2-3
            if (banner.version >= 2) bw.Write(StringToTitle(banner.chineseTitle));
            if (banner.version >= 3) bw.Write(StringToTitle(banner.koreanTitle));
            // DSi Enchansed
            if ((banner.version >> 8) == 1)
            {
                bw.Write(banner.reservedDsi);
                //byte[] zbyte = new byte[0x800];
                //bw.Write(zbyte);
                bw.Write(banner.aniIconData);
            }

            uint size = (uint)bw.BaseStream.Position;
            int rem = (int)bw.BaseStream.Position % 0x200;
            // Write padding bytes...
            while (rem < 0x200)
            {
                bw.Write((byte)0xFF);
                rem++;
            }
            
            Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S09"), bw.BaseStream.Length);
            bw.Flush();
            bw.Close();
            return size;
        }
        public static string TitleToString(byte[] data)
        {
            string title = "";
            title = new String(Encoding.Unicode.GetChars(data));
            title = title.Replace("\n", "\r\n");

            return title;
        }
        public static byte[] StringToTitle(string title)
        {
            List<byte> data = new List<byte>();

            title = title.Replace("\r", "");
            data.AddRange(Encoding.Unicode.GetBytes(title));

            int relleno = 0x100 - data.Count;
            for (int i = 0; i < relleno; i++)
                data.Add(0x00);

            return data.ToArray();
        }
        public static Bitmap IconoToBitmap(byte[] tileData, byte[] paletteData)
        {
            Bitmap imagen = new Bitmap(32, 32);
            Color[] paleta = Actions.BGR555ToColor(paletteData);

            tileData = BitsConverter.BytesToBit4(tileData);
            int i = 0;
            for (int hi = 0; hi < 4; hi++)
            {
                for (int wi = 0; wi < 4; wi++)
                {
                    for (int h = 0; h < 8; h++)
                    {
                        for (int w = 0; w < 8; w++)
                        {
                            imagen.SetPixel(w + wi * 8, h + hi * 8, paleta[tileData[i]]);
                            i++;
                        }
                    }
                }
            }

            return imagen;
        }

        public static void Write_Files(string fileOut, string romFile, sFolder root, ushort[] sortedIDs)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileOut));
            BinaryReader br = new BinaryReader(File.OpenRead(romFile));

            // Get overlays IDs (all system files)
            int sysIndex = 0;
            ushort[] ovIDs = new ushort[root.folders[root.folders.Count - 1].files.Count];
            for (int i = 0; i < ovIDs.Length; i++) ovIDs[i] = root.folders[root.folders.Count - 1].files[i].id;
            Array.Sort(ovIDs);

            Console.Write(Tools.Helper.GetTranslation("Messages", "S0B"));

            for (int i = 0; i < sortedIDs.Length; i++)
            {
                if (i == 0 & sortedIDs[i] > sortedIDs.Length)
                    continue;

                if (sortedIDs[i] == ovIDs[sysIndex])
                {
                    // Exclude overlay files by ID, because some files have name with "overlay" on begin
                    sysIndex++;
                    continue;
                }

                sFile currFile = BuscarArchivo(sortedIDs[i], root);
                if (!(currFile.name is string)) continue;

                if (currFile.path == romFile)
                {
                    br.BaseStream.Position = currFile.offset;
                    bw.Write(br.ReadBytes((int)currFile.size));
                    bw.Flush();
                }
                else
                {
                    BinaryReader br2 = new BinaryReader(File.OpenRead(currFile.path));
                    br2.BaseStream.Position = currFile.offset;
                    bw.Write(br2.ReadBytes((int)currFile.size));

                    br2.Close();
                    bw.Flush();
                }

                // Padd for next file.
                // There is no need to padd last file since no more data will be
                // after it. A full padding of the ROM will be applied later.
                int rem = (int)bw.BaseStream.Position % 0x200;
                if (rem != 0 && i != sortedIDs.Length - 1)
                {
                    while (rem < 0x200)
                    {
                        bw.Write((byte)0xFF);
                        rem++;
                    }
                }
            }

            bw.Flush();
            bw.Close();
            br.Close();
            Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S0C"), sortedIDs.Length);
        }
        private static sFile BuscarArchivo(int id, sFolder currFolder)
        {
            if (currFolder.id == id) // Archivos descomprimidos
            {
                sFile folderFile = new sFile();
                folderFile.name = currFolder.name;
                folderFile.id = currFolder.id;
                folderFile.size = Convert.ToUInt32(((String)currFolder.tag).Substring(0, 8), 16);
                folderFile.offset = Convert.ToUInt32(((String)currFolder.tag).Substring(8, 8), 16);
                folderFile.path = ((string)currFolder.tag).Substring(16);

                return folderFile;
            }

            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.id == id)
                        return archivo;


            if (currFolder.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in currFolder.folders)
                {
                    sFile currFile = BuscarArchivo(id, subFolder);
                    if (currFile.name is string)
                        return currFile;
                }
            }

            return new sFile();
        }

        private static void Rellenar_MakerCodes()
        {
            Estructuras.makerCode = new Dictionary<string, string>();
            Dictionary<string, string> diccionario = Estructuras.makerCode;

            diccionario.Add("01", "Nintendo");
            diccionario.Add("02", "Rocket Games");
            diccionario.Add("03", "Imagineer Zoom");
            diccionario.Add("04", "Gray Matter");
            diccionario.Add("05", "Zamuse");
            diccionario.Add("06", "Falcom");
            diccionario.Add("07", "Enix");
            diccionario.Add("08", "Capcom");
            diccionario.Add("09", "Hot B");
            diccionario.Add("0A", "Jaleco");

            diccionario.Add("13", "Electronic Arts Japan");
            diccionario.Add("18", "Hudson Entertainment");
            diccionario.Add("20", "Destination Software");
            diccionario.Add("36", "Codemasters");
            diccionario.Add("41", "Ubisoft");
            diccionario.Add("4A", "Gakken");
            diccionario.Add("4F", "Eidos");
            diccionario.Add("4Q", "Disney Interactive Studios");
            diccionario.Add("4Z", "Crave Entertainment");
            diccionario.Add("52", "Activision");
            diccionario.Add("54", "ROCKSTAR GAMES");
            diccionario.Add("5D", "Midway");
            diccionario.Add("5G", "Majesco Entertainment");
            diccionario.Add("64", "LucasArts Entertainment");
            diccionario.Add("69", "Electronic Arts Inc.");
            diccionario.Add("6K", "UFO Interactive");
            diccionario.Add("6V", "JoWooD Entertainment");
            diccionario.Add("70", "Atari");
            diccionario.Add("78", "THQ");
            diccionario.Add("7D", "Vivendi Universal Games");
            diccionario.Add("7J", "Zoo Digital Publishing Ltd");
            diccionario.Add("7N", "Empire Interactive");
            diccionario.Add("7U", "Ignition Entertainment");
            diccionario.Add("7V", "Summitsoft Entertainment");
            diccionario.Add("8J", "General Entertainment");
            diccionario.Add("8P", "SEGA");
            diccionario.Add("99", "Rising Star Games");
            diccionario.Add("A4", "Konami Digital Entertainment");
            diccionario.Add("AF", "Namco");
            diccionario.Add("B2", "Bandai");
            diccionario.Add("E9", "Natsume");
            diccionario.Add("EB", "Atlus");
            diccionario.Add("FH", "Foreign Media Games");
            diccionario.Add("FK", "The Game Factory");
            diccionario.Add("FP", "Mastiff");
            diccionario.Add("FQ", "iQue");
            diccionario.Add("FR", "dtp young");
            diccionario.Add("G9", "D3Publisher of America");
            diccionario.Add("GD", "SQUARE ENIX");
            diccionario.Add("GL", "gameloft");
            diccionario.Add("GN", "Oxygen Interactive");
            diccionario.Add("GR", "GSP");
            diccionario.Add("GT", "505 Games");
            diccionario.Add("GQ", "Engine Software");
            diccionario.Add("GY", "The Game Factory");
            diccionario.Add("H3", "Zen");
            diccionario.Add("H4", "SNK PLAYMORE");
            diccionario.Add("H6", "MYCOM");
            diccionario.Add("HC", "Plato");
            diccionario.Add("HF", "Level 5");
            diccionario.Add("HG", "Graffiti Entertainment");
            diccionario.Add("HM", "HMH - INTERACTIVE");
            diccionario.Add("HV", "bhv Software GmbH");
            diccionario.Add("LR", "Asylum Entertainment");
            diccionario.Add("KJ", "Gamebridge");
            diccionario.Add("KM", "Deep Silver");
            diccionario.Add("MJ", "MumboJumbo");
            diccionario.Add("MT", "Blast Entertainment");
            diccionario.Add("NK", "Neko Entertainment");
            diccionario.Add("NP", "Nobilis Publishing");
            diccionario.Add("PG", "Phoenix Games");
            diccionario.Add("PL", "Playlogic");
            diccionario.Add("SU", "Slitherine Software UK Ltd");
            diccionario.Add("SV", "SevenOne Intermedia GmbH");
            diccionario.Add("RM", "rondomedia");
            diccionario.Add("RT", "RTL Games");
            diccionario.Add("TK", "Tasuke");
            diccionario.Add("TR", "Tetris Online");
            diccionario.Add("TV", "Tivola Publishing");
            diccionario.Add("VP", "Virgin Play");
            diccionario.Add("WP", "White Park Bay");
            diccionario.Add("WR", "Warner Bros");
            diccionario.Add("XS", "Aksys Games");
        }
        private static void Rellenar_UnitCodes()
        {
            Estructuras.unitCode = new Dictionary<byte, string>();
            Estructuras.unitCode.Add(0x00, "Nintendo DS");
            Estructuras.unitCode.Add(0x02, "Nintendo DSi Enhanced");
            Estructuras.unitCode.Add(0x03, "Nintendo DSi");
        }
    }
}
