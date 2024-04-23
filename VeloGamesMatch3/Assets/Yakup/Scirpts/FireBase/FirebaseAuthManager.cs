using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Security.Policy;
using System;

public class FirebaseAuthManager : MonoBehaviour
{
    // Firebase variable

    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    // Login Variables
    [Space]
    [Header("Login")]
    public InputField emailLoginField;
    public InputField passwordLoginField;

    // Registration Variables
    [Space]
    [Header("Registration")]
    public InputField nameRegisterField;
    public InputField emailRegisterField;
    public InputField passwordRegisterField;
    public InputField confirmPasswordRegisterField;

    private string defaultUserImage = "https://www.esportimes.com/wp-content/uploads/2022/08/esportimeslogo-optimized.png";

    public static FirebaseAuthManager Instance;

    private static FirebaseAuthManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            // İlk oluşturulan örneği işaretle
            instance = this;

            // Bu nesnenin yok edilmemesini sağla
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Birden fazla örnek oluşursa bu nesneyi yok et
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // Firebase bağımlılıklarını başlat
        StartCoroutine(CheckAndFixDependenciesAsync());
    }

    private IEnumerator CheckAndFixDependenciesAsync()
    {
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => dependencyTask.IsCompleted);
        dependencyStatus = dependencyTask.Result;

        if (dependencyStatus == DependencyStatus.Available)
        {
            InitializeFirebase();
            yield return new WaitForEndOfFrame();
            StartCoroutine(CheckForAutoLogin());
        }
        else
        {
            Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
            UIManager.Instance.ErrorTxT.text = "Could not resolve all firebase dependencies: " + dependencyStatus;

        }
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private IEnumerator CheckForAutoLogin()
    {
        if (user != null)
        {
            var RelodUserTask = user.ReloadAsync();
            yield return new WaitUntil(() => RelodUserTask.IsCompleted);
            AutoLogin();
        }
        else
        {
            UIManager.Instance.OpenLoginPanel();
        }
    }

    private void AutoLogin()
    {
        if (user != null)
        {
            if (user.IsEmailVerified)
            {
                References.userName = user.DisplayName;
                UIManager.Instance.OpenGamePanel();

                // Check if PhotoUrl is not null before converting it to string
                if (user.PhotoUrl != null && !string.IsNullOrEmpty(user.PhotoUrl.ToString()))
                {
                    UIManager.Instance.LoadProfileImage(user.PhotoUrl.ToString());
                }
            }
            else
            {
                SendEmailForVerifaction();
            }
        }
        else
        {
            UIManager.Instance.OpenLoginPanel();
        }
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                UIManager.Instance.OpenLoginPanel();
                ClearLoginInputFieldText();
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    private void ClearLoginInputFieldText()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    public void Login()
    {
        StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
    }

    public void Logout()
    {
        if (auth != null && user != null)
        {
            auth.SignOut();
        }
    }

    private IEnumerator LoginAsync(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogError(loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            string failedMessage = "Login Failed! Because ";

            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failedMessage += "Email is invalid";
                    break;
                case AuthError.WrongPassword:
                    failedMessage += "Wrong Password";
                    break;
                case AuthError.MissingEmail:
                    failedMessage += "Email is missing";
                    break;
                case AuthError.MissingPassword:
                    failedMessage += "Password is missing";
                    break;
                default:
                    failedMessage = "Login Failed";
                    break;
            }

            Debug.Log(failedMessage);
            UIManager.Instance.ErrorTxT.text = failedMessage;

        }
        else
        {
            Firebase.Auth.AuthResult authResult = loginTask.Result;
            FirebaseUser newUser = authResult.User;

            Debug.LogFormat("{0} You Are Successfully Logged In", user.DisplayName);
            if (user.IsEmailVerified)
            {
                References.userName = user.DisplayName;
                UIManager.Instance.OpenGamePanel();

                if (!string.IsNullOrEmpty(user.PhotoUrl.ToString()))
                {
                    UIManager.Instance.LoadProfileImage(user.PhotoUrl.ToString());
                }
            }
            else
            {
                SendEmailForVerifaction();
            }
        }
    }

    public void Register()
    {
        StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text));
    }

    private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword)
    {
        if (name == "")
        {
            Debug.LogError("User Name is empty");
            UIManager.Instance.ErrorTxT.text = "User Name is empty";
        }
        else if (email == "")
        {
            Debug.LogError("email field is empty");
            UIManager.Instance.ErrorTxT.text = "email field is empty";
        }
        else if (passwordRegisterField.text != confirmPasswordRegisterField.text)
        {
            Debug.LogError("Password does not match");
            UIManager.Instance.ErrorTxT.text = "Password does not match";
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                Debug.LogError(registerTask.Exception);

                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Registration Failed! Because ";
                switch (authError)
                {
                    case AuthError.InvalidEmail:
                        failedMessage += "Email is invalid";
                        break;
                    case AuthError.WrongPassword:
                        failedMessage += "Wrong Password";
                        break;
                    case AuthError.MissingEmail:
                        failedMessage += "Email is missing";
                        break;
                    case AuthError.MissingPassword:
                        failedMessage += "Password is missing";
                        break;
                    default:
                        failedMessage = "Registration Failed";
                        break;
                }

                Debug.Log(failedMessage);
                UIManager.Instance.ErrorTxT.text = failedMessage;
            }
            else
            {
                // bilgileri al
                Firebase.Auth.AuthResult authResult = registerTask.Result;
                FirebaseUser newUser = authResult.User;

                UserProfile userProfile = new UserProfile { DisplayName = name, PhotoUrl = new Uri(defaultUserImage) };

                var updateProfileTask = newUser.UpdateUserProfileAsync(userProfile);

                yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                if (updateProfileTask.Exception != null)
                {
                    // Kullanıcı oluşturuldu ancak profil güncellenemedi, bu nedenle kullanıcıyı sil
                    newUser.DeleteAsync();

                    Debug.LogError(updateProfileTask.Exception);

                    FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;

                    string failedMessage = "Profile update Failed! Because ";
                    switch (authError)
                    {
                        case AuthError.InvalidEmail:
                            failedMessage += "Email is invalid";
                            break;
                        case AuthError.WrongPassword:
                            failedMessage += "Wrong Password";
                            break;
                        case AuthError.MissingEmail:
                            failedMessage += "Email is missing";
                            break;
                        case AuthError.MissingPassword:
                            failedMessage += "Password is missing";
                            break;
                        default:
                            failedMessage = "Profile update Failed";
                            break;
                    }

                    Debug.Log(failedMessage);
                }
                else
                {
                    Debug.Log("Registration Successful. Welcome " + newUser.DisplayName);
                    if (user.IsEmailVerified)
                    {
                        UIManager.Instance.OpenLoginPanel();
                    }
                    else
                    {
                        SendEmailForVerifaction();
                    }
                }
            }
        }
    }

    public void SendEmailForVerifaction()
    {
        StartCoroutine(SendEmailVerifactionAsync());
    }

    private IEnumerator SendEmailVerifactionAsync()
    {
        if (user != null)
        {
            var sendEmilask = user.SendEmailVerificationAsync();

            yield return new WaitUntil(() => sendEmilask.IsCompleted);
            if (sendEmilask.Exception != null)
            {
                FirebaseException firebaseException = sendEmilask.Exception.GetBaseException() as FirebaseException;
                AuthError error = (AuthError)firebaseException.ErrorCode;
                string errorMessage = "Unknown Error : Please try again later";
                switch (error)
                {
                    case AuthError.Cancelled:
                        errorMessage = "Email verification was Cancelled";
                        break;
                    case AuthError.TooManyRequests:
                        errorMessage = "Too Many Request";
                        break;
                    case AuthError.InvalidRecipientEmail:
                        errorMessage = "The Email You Entered is Invalid";
                        break;
                }

                UIManager.Instance.ShowVerificationResponse(false, user.Email, errorMessage);
            }
            else
            {
                Debug.Log("Email has successfully sent");
                UIManager.Instance.ShowVerificationResponse(true, user.Email, null);
            }
        }
    }

    public void UpdateProfilePicture()
    {
        StartCoroutine(UpdateProfilePictureIE());
    }

    private IEnumerator UpdateProfilePictureIE()
    {
        if (user != null)
        {
            string url = UIManager.Instance.GetUpdateURL();

            // Check if the URL string is valid before creating a Uri object
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                UserProfile profile = new UserProfile() { PhotoUrl = uri };

                var profileUpdateTask = user.UpdateUserProfileAsync(profile);
                yield return new WaitUntil(() => profileUpdateTask.IsCompleted);

                if (profileUpdateTask.Exception != null)
                {
                    Debug.LogError(profileUpdateTask.Exception);
                }
                else
                {
                    // Update the profile image if the profile update is successful
                    UIManager.Instance.LoadProfileImage(user.PhotoUrl.ToString());
                }
            }
            else
            {
                Debug.LogError("Invalid URL format:" + url);
                UIManager.Instance.ErrorTxT.text = "Invalid URL format:" + url;
            }
        }
    }

    public void OpenGameScene()
    {
        SceneManager.LoadScene(1);

    }
}
