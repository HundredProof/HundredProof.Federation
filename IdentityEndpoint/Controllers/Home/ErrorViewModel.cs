using IdentityServer4.Models;

namespace IdentityEndpoint.Controllers.Home {
    public class ErrorViewModel {
        public ErrorViewModel() { }

        public ErrorViewModel(string error) {
            Error = new ErrorMessage {Error = error};
        }

        public ErrorMessage Error { get; set; }
    }
}