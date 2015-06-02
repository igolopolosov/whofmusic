//-----------------------------------------------------------------------
// <copyright file="App.xaml.cs">
//     Copyright (c) Igor Golopolosov. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using WarehouseOfMusic.Managers;
using WomAudioComponent;

namespace WarehouseOfMusic
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Models;
    using Resources;

    /// <summary>
    /// Class of an application
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Specify the local database connection string.
        /// </summary>
        public const string DbConnectionString = "Data Source=isostore:/WarehouseOfMusic.sdf";

        /// <summary>
        /// Avoid double initialization
        /// </summary>
        private bool _phoneApplicationInitialized;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="App" /> class.
        /// Constructor of an object of application.
        /// </summary>
        public App()
        {
            // Глобальный обработчик неперехваченных исключений.
            this.UnhandledException += this.Application_UnhandledException;

            // Стандартная инициализация XAML
            this.InitializeComponent();

            // Инициализация телефона
            this.InitializePhoneApplication();

            // Инициализация отображения языка
            this.InitializeLanguage();

            // Отображение сведений о профиле графики во время отладки.
            if (Debugger.IsAttached)
            {
                // Отображение текущих счетчиков частоты смены кадров.
                Current.Host.Settings.EnableFrameRateCounter = true;

                // Отображение областей приложения, перерисовываемых в каждом кадре.
                // Application.Current.Host.Settings.EnableRedrawRegions = true;
                // Включение режима визуализации анализа нерабочего кода,
                // для отображения областей страницы, переданных в GPU, с цветным наложением.
                // Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Предотвратить выключение экрана в режиме отладчика путем отключения
                // определения состояния простоя приложения.
                // Внимание! Используйте только в режиме отладки. Приложение, в котором отключено обнаружение бездействия пользователя, будет продолжать работать
                // и потреблять энергию батареи, когда телефон не будет использоваться.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            // Create the database if it does not exist.
            using (var db = new ToDoDataContext(DbConnectionString))
            {
                if (db.DatabaseExists() == false)
                {
                    // Create the local database.
                    db.CreateDatabase();
                    db.Populate();
                }
            }

            // Initialize common audiocontroller for app
            PlayerManager.InitializeAudioController();
        }

        /// <summary>
        /// Gets easy access to root frame of application.
        /// </summary>
        /// <returns>Root frame of application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// The code that is executed if the contract is activated, such as opening a file or a selection of files in the save window, 
        /// returns the selected file or other return values
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Contract is activated</param>
        private void Application_ContractActivated(object sender, Windows.ApplicationModel.Activation.IActivatedEventArgs e)
        {
        }

        /// <summary>
        /// The code to execute when the application starts (for example, from "Start" menu) 
        /// This code will not execute when the application re-activation
        /// /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Launch of an application</param>
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        /// <summary>
        /// The code to execute when the application is activated (translated in Native Mode)
        /// This code will not run when you first start the application
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Application is activated</param>
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        /// <summary>
        /// The code to execute when the deactivation application (sent to background)
        /// This code will not execute when the application is closed
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Deactivation of an application</param>
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        /// <summary>
        /// The code to execute when the application is closed (for example, when the user clicks the "Back")
        /// This code will not execute when the application deactivation
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Application is closed</param>
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        /// <summary>
        /// The code to execute if an error navigation
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Error of navigation</param>
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // Ошибка навигации; перейти в отладчик
                Debugger.Break();
            }
        }

        /// <summary>
        /// Code for execution on unhandled exception
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Unhandled exception</param>
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // Произошло необработанное исключение; перейти в отладчик
                Debugger.Break();
            }
        }

        #region Инициализация приложения телефона

        /// <summary>
        /// Do not add additional code, this method
        /// </summary>
        private void InitializePhoneApplication()
        {
            if (this._phoneApplicationInitialized)
            {
                return;
            }

            // Создайте кадр, но не задавайте для него значение RootVisual; это позволит
            // экрану-заставке оставаться активным, пока приложение не будет готово для визуализации.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += this.CompleteInitializePhoneApplication;

            // Обработка сбоев навигации
            RootFrame.NavigationFailed += this.RootFrame_NavigationFailed;

            // Обработка запросов на сброс для очистки стека переходов назад
            RootFrame.Navigated += this.CheckForResetNavigation;

            // Обработка активации контракта, такого как открытие файла или выбор файлов в окне сохранения
            PhoneApplicationService.Current.ContractActivated += this.Application_ContractActivated;

            // Убедитесь, что инициализация не выполняется повторно
            this._phoneApplicationInitialized = true;
        }

        /// <summary>
        /// Do not add additional code, this method
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Complete initialize phone application</param>
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Задайте корневой визуальный элемент для визуализации приложения
            if (RootVisual != RootFrame)
            {
                this.RootVisual = RootFrame;
            }

            // Удалите этот обработчик, т.к. он больше не нужен
            RootFrame.Navigated -= this.CompleteInitializePhoneApplication;
        }

        /// <summary>
        /// If the application has a navigation "reset", should be checked
        /// The next navigation to check whether you need to reset the stack
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Next Navigation</param>
        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Reset)
            {
                RootFrame.Navigated += this.ClearBackStackAfterReset;
            }
        }

        /// <summary>
        /// Clear of back stack
        /// </summary>
        /// <param name="sender">Some object</param>
        /// <param name="e">Next Navigation</param>
        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Отменить регистрацию события, чтобы оно больше не вызывалось
            RootFrame.Navigated -= this.ClearBackStackAfterReset;

            // Очистка стека только для "новых" навигаций (вперед) и навигаций "обновления"
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
            {
                return;
            }

            // Очистка всего стека страницы для согласованности пользовательского интерфейса
            while (RootFrame.RemoveBackEntry() != null)
            {
                // ничего не делать
            }
        }

        #endregion

        // Инициализация шрифта приложения и направления текста, как определено в локализованных строках ресурсов.
        //
        // Чтобы убедиться, что шрифт приложения соответствует поддерживаемым языкам, а
        // FlowDirection для каждого из этих языков соответствует традиционному направлению, ResourceLanguage
        // и ResourceFlowDirection необходимо инициализировать в каждом RESX-файле, чтобы эти значения совпадали с
        // культурой файла. Пример:
        //
        // AppResources.es-ES.resx
        //    Значение ResourceLanguage должно равняться "es-ES"
        //    Значение ResourceFlowDirection должно равняться "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     Значение ResourceLanguage должно равняться "ar-SA"
        //     Значение ResourceFlowDirection должно равняться "RightToLeft"
        //
        //// Дополнительные сведения о локализации приложений Windows Phone см. на странице http://go.microsoft.com/fwlink/?LinkId=262072.

        /// <summary>
        /// Initialize the application's font and text direction as defined in the localized resource strings.
        /// </summary>
        private void InitializeLanguage()
        {
            try
            {
                // Задать шрифт в соответствии с отображаемым языком, определенным
                // строкой ресурса ResourceLanguage для каждого поддерживаемого языка.
                //
                // Откат к шрифту нейтрального языка, если отображаемый
                // язык телефона не поддерживается.
                //
                // Если возникла ошибка компилятора, ResourceLanguage отсутствует в
                // файл ресурсов.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Установка FlowDirection для всех элементов в корневом кадре на основании
                // строки ресурса ResourceFlowDirection для каждого
                // поддерживаемого языка.
                //
                // Если возникла ошибка компилятора, ResourceFlowDirection отсутствует в
                // файл ресурсов.
                var flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // Если здесь перехвачено исключение, вероятнее всего это означает, что
                // для ResourceLangauge неправильно задан код поддерживаемого языка
                // или для ResourceFlowDirection задано значение, отличное от LeftToRight
                //// или RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}