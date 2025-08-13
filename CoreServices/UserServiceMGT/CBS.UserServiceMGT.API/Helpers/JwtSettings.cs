// Source: D�duit de votre appsettings.json mod�le et de la logique de validation JWT.
namespace CBS.UserServiceMGT.API.Helpers
{
    public class JwtSettings
    {
        /// <summary>
        /// La cl� secr�te utilis�e pour signer et valider le token.
        /// Doit �tre suffisamment longue et complexe.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// L'�metteur du token (l'autorit� qui a g�n�r� le token).
        /// Ex: "https://identity.bapcculcbs.com/"
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// L'audience attendue du token (le service � qui le token est destin�).
        /// Ex: "CBS.UserServiceMGT"
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// La dur�e de validit� du token en minutes.
        /// </summary>
        public int MinutesToExpiration { get; set; }
    }
}