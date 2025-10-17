using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;

/// <summary>
/// Gestor principal de localización para el juego
/// Maneja el cambio de idiomas y persistencia de preferencias
/// </summary>
public class LocalizationManager : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Cargar automáticamente el idioma guardado al iniciar")]
    public bool loadSavedLanguageOnStart = true;

    [Tooltip("Idioma por defecto si no hay uno guardado")]
    public string defaultLanguageCode = "es"; // español por defecto

    private const string LANGUAGE_PREF_KEY = "SelectedLanguage";

    private void Start()
    {
        if (loadSavedLanguageOnStart)
        {
            StartCoroutine(LoadSavedLanguage());
        }
    }

    /// <summary>
    /// Cambia el idioma del juego
    /// </summary>
    /// <param name="languageCode">Código del idioma (ej: 'en', 'es')</param>
    public void ChangeLanguage(string languageCode)
    {
        StartCoroutine(SetLanguage(languageCode));
    }

    /// <summary>
    /// Cambia el idioma a inglés
    /// </summary>
    public void ChangeToEnglish()
    {
        ChangeLanguage("en");
    }

    /// <summary>
    /// Cambia el idioma a español
    /// </summary>
    public void ChangeToSpanish()
    {
        ChangeLanguage("es");
    }

    /// <summary>
    /// Obtiene el código del idioma actual
    /// </summary>
    public string GetCurrentLanguage()
    {
        if (LocalizationSettings.SelectedLocale != null)
        {
            return LocalizationSettings.SelectedLocale.Identifier.Code;
        }
        return defaultLanguageCode;
    }

    private IEnumerator SetLanguage(string languageCode)
    {
        // Esperar a que el sistema de localización esté inicializado
        yield return LocalizationSettings.InitializationOperation;

        // Buscar el locale correspondiente
        var locale = LocalizationSettings.AvailableLocales.GetLocale(languageCode);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            
            // Guardar la preferencia
            PlayerPrefs.SetString(LANGUAGE_PREF_KEY, languageCode);
            PlayerPrefs.Save();

            Debug.Log($"Idioma cambiado a: {languageCode}");

            // IMPORTANTE: Forzar actualización de todos los componentes de localización
            yield return new WaitForSeconds(0.1f);
            RefreshAllLocalizedComponents();
        }
        else
        {
            Debug.LogWarning($"No se encontró el idioma con código: {languageCode}");
        }
    }

    private IEnumerator LoadSavedLanguage()
    {
        // Esperar a que el sistema de localización esté inicializado
        yield return LocalizationSettings.InitializationOperation;

        string savedLanguage = PlayerPrefs.GetString(LANGUAGE_PREF_KEY, "");

        if (string.IsNullOrEmpty(savedLanguage))
        {
            // Si no hay idioma guardado, usar el idioma del sistema o el por defecto
            var systemLanguage = Application.systemLanguage;
            savedLanguage = (systemLanguage == SystemLanguage.Spanish) ? "es" : defaultLanguageCode;
        }

        // Cambiar al idioma guardado
        yield return SetLanguage(savedLanguage);
    }

    /// <summary>
    /// Fuerza la actualización de todos los componentes Localize String Event en la escena
    /// </summary>
    private void RefreshAllLocalizedComponents()
    {
        // Encontrar todos los componentes LocalizeStringEvent en la escena
        var localizedComponents = FindObjectsOfType<UnityEngine.Localization.Components.LocalizeStringEvent>();
        
        foreach (var component in localizedComponents)
        {
            // Forzar la actualización del componente
            component.RefreshString();
        }

        Debug.Log($"Se actualizaron {localizedComponents.Length} componentes de localización");
    }

    /// <summary>
    /// Método público para refrescar manualmente todos los textos
    /// </summary>
    public void RefreshAllTexts()
    {
        RefreshAllLocalizedComponents();
    }
}
