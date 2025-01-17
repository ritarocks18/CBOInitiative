﻿using AAPS.L10nPortal.Bal.AzureBlob;
using AAPS.L10nPortal.Bal.Exceptions;
using AAPS.L10nPortal.Contracts.Managers;
using AAPS.L10nPortal.Contracts.Models;
using AAPS.L10nPortal.Contracts.Repositories;
using AAPS.L10nPortal.Dal.Exceptions;
using AAPS.L10nPortal.Entities;
using System.Text.RegularExpressions;

namespace AAPS.L10nPortal.Bal
{
    public class ApplicationLocaleAssetManager : IApplicationLocaleAssetManager
    {
        private readonly IApplicationLocaleAssetRepository applicationLocaleAssetRepository;
        private readonly IApplicationLocaleRepository applicationLocaleRepository;

        public ApplicationLocaleAssetManager(IApplicationLocaleAssetRepository applicationLocaleAssetRepository, IApplicationLocaleRepository applicationLocaleRepository)
        {
            this.applicationLocaleAssetRepository = applicationLocaleAssetRepository;
            this.applicationLocaleRepository = applicationLocaleRepository;
        }
        public static string PadNumbers(string input)
        {
            return Regex.Replace(input, "[0-9]+", match => match.Value.PadLeft(10, '0'));
        }
        public async Task<ApplicationAssets> GetListAsync(PermissionData permissionData, int applicationLocaleId)
        {
            try
            {
                var applicationLocale =
                    (applicationLocaleRepository.GetUserApplicationLocaleById(permissionData, applicationLocaleId))
                    .FirstOrDefault();

                return new ApplicationAssets
                {
                    ApplicationName = applicationLocale.ApplicationName,
                    LocaleCode = applicationLocale.LocaleCode,
                    Assets = null// (await applicationLocaleAssetRepository.GetList(permissionData, applicationLocaleId)).OrderBy(x => PadNumbers(x.Key))
                };
            }
            catch (PermissionException)
            {
                throw new ApplicationLocaleDeniedException();
            }
        }

        public async Task<ApplicationAssets> GetAssetKeysWithAssets(PermissionData permissionData, int applicationLocaleId)
        {
            try
            {
                var applicationLocale =
                    (applicationLocaleRepository.GetUserApplicationLocaleById(permissionData, applicationLocaleId))
                    .FirstOrDefault();

                return new ApplicationAssets
                {
                    ApplicationName = applicationLocale.ApplicationName,
                    LocaleCode = applicationLocale.LocaleCode,
                    Assets = null// (await applicationLocaleAssetRepository.GetAssetKeyWithAsset(permissionData, applicationLocaleId)).Select(x => x.MapTo<Asset>()).OrderBy(x => PadNumbers(x.Key))
                };
            }
            catch (PermissionException)
            {
                throw new ApplicationLocaleDeniedException();
            }
        }

        public async Task<DownloadAssetFileResult> Download(PermissionData permissionData, int applicationLocaleId, int keyId)
        {
            try
            {
                var applicationLocale =
                    (applicationLocaleRepository.GetUserApplicationLocaleById(permissionData, applicationLocaleId))
                    .FirstOrDefault();

                var asset = "test";
                //(await applicationLocaleAssetRepository.Get(permissionData, applicationLocaleId, keyId))
                //    .FirstOrDefault()
                //    .MapTo<Asset>();

                if (applicationLocale == null)
                    throw new BadRequestException("Requested application locale does not exists");

                if (asset == null)
                    throw new BadRequestException("Requested asset with the given key does not exists");

                var blobName = "test";// BuildAssetBlobName(applicationLocale, asset, asset);

                return await BlobService.DownloadFile(blobName, asset);
            }
            catch (PermissionException)
            {
                throw new ApplicationLocaleDeniedException();
            }
            catch (Exception ex)
            {
                throw new AssetDownloadException(ex);
            }
        }

        public async Task<Asset> UploadAsync(PermissionData permissionData, int applicationLocaleId, int key, string fileName, Stream inputStream)
        {
            try
            {
                var applicationLocale =
                    (applicationLocaleRepository.GetUserApplicationLocaleById(permissionData, applicationLocaleId))
                    .FirstOrDefault();

                var asset = "test";
                //(await applicationLocaleAssetRepository.Get(permissionData, applicationLocaleId, key))
                //    .FirstOrDefault()
                //    .MapTo<Asset>();

                if (applicationLocale == null)
                    throw new BadRequestException("Requested application locale does not exists");

                if (asset == null)
                    throw new BadRequestException("Requested asset with the given key does not exists");

                var blobName = "test";// BuildAssetBlobName(applicationLocale, asset, fileName);

                var uri = await BlobService.UploadFileAsync(blobName, inputStream);

                return null;
                //(await applicationLocaleAssetRepository.Update(
                //    permissionData, applicationLocaleId, key, uri.ToString()))
                //.FirstOrDefault()
                //.MapTo<Asset>();
            }
            catch (PermissionException)
            {
                throw new ApplicationLocaleDeniedException();
            }
            catch (Exception ex)
            {
                throw new AssetUploadException(ex);
            }
        }

        private string BuildAssetBlobName(UserApplicationLocale applicationLocale, Asset asset, string fileName)
        {
            return $"{applicationLocale.ApplicationName}.[{asset.Key}].{applicationLocale.LocaleCode}{Path.GetExtension(fileName)}";
        }
    }
}