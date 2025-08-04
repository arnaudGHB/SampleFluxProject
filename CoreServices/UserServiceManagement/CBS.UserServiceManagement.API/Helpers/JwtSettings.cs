// Source: Déduit de votre appsettings.json modèle et de la logique de validation JWT.
namespace CBS.UserServiceManagement.API.Helpers
{
    public class JwtSettings
    {
        /// <summary>
        /// La clé secrète utilisée pour signer et valider le token.
        /// Doit être suffisamment longue et complexe.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// L'émetteur du token (l'autorité qui a généré le token).
        /// Ex: "https://identity.bapcculcbs.com/"
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// L'audience attendue du token (le service à qui le token est destiné).
        /// Ex: "CBS.UserServiceManagement"
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// La durée de validité du token en minutes.
        /// </summary>
        public int MinutesToExpiration { get; set; }
    }
}