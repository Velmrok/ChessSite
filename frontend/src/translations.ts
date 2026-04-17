import { create } from "domain";
import { changePassword } from "./services/userService";


export const translations = {
  // pl: {
  //   friendAdded: "Znajomy dodany pomyślnie!",
  //   friendAddError: "Nie udało się dodać znajomego.",
  //   friendDeleted: "Znajomy usunięty pomyślnie!",
  //   game: {
  //     chat: {
  //       title: "Czat gry",
  //       send: "Wyślij",
  //       placeholder: "Napisz coś..."
  //     },
  //     title:{
  //       won:  "Wygrana!",
  //     lost: "Przegrana!",
  //     youWon: "Wygrałeś!",
  //     youLost: "Przegrałeś!",
  //     draw: "Remis!",
  //     },
  //     reason:{
  //       threefoldRepetition: "Remis przez potrójne powtórzenie",
  //       insufficientMaterial: "Remis przez niewystarczający materiał",
  //       timeout: "Przekroczenie czasu",
  //       surrender: "Poddanie się",
  //       agreement: "Remis przez porozumienie",
  //       stalemate: "Remis przez pat",
  //       checkmate: "Wygrana przez mata",
  //       disconnect: "Wygrana przez rozłączenie przeciwnika"
  //     },
  //     offerDraw: "Zaproponuj remis",
  //     drawOffered: "Remis zaproponowany",
  //     surrender: "Poddaj się",
  //     getPGN: "Pobierz PGN",
  //     acceptDraw: "Zaakceptuj remis"
  //   },
  //   notFound: {
  //     title: "Nie znaleziono 404",
  //     backToHome: "Wróć do strony głównej",
  //     user: "Nie znaleziono użytkownika",
  //     game: "Nie znaleziono gry",
  //     page: "Nie znaleziono strony"
  //   },
  //   navbar: {
  //     login: "Zaloguj się",
  //     register: "Zarejestruj się",
  //     logout: "Wyloguj się",
  //     inQueue: "W kolejce",
  //     leaveQueue: "Opuść kolejkę"

  //   },
  //   profile: {
  //     bio: {
  //       save: "Zapisz",
  //       cancel: "Anuluj",
  //       edit: "Edytuj opis"
  //     },
  //     avatar: {
  //       change: "Zmień avatar"
  //     },
  //     userInfo: {
  //       rating: "Ranking",
  //       joinDate: "Data dołączenia",
  //       gamesPlayed: "Gry",
  //       wins: "Wygrane",
  //       losses: "Przegrane",
  //       draws: "Remisy"
  //     },
  //     notfound: "Nie znaleziono użytkownika",
  //     backToHome: "Wróć do strony głównej",
  //     addFriend: "Dodaj znajomego",
  //     deleteFriend: "Usuń znajomego",
  //     gameHistory: "Historia gier",
  //     friendList: "Lista znajomych",
  //     noFriends: "Brak znajomych",
  //     friends: "Znajomi",
  //     online: "Online",
  //     offline: "Offline",
  //     delete: "Konto zostało usunięte.",
  //     deleteAccount: "Usuń konto",
  //     rating: "Ranking",
  //     eloChart: "Wykres Elo",
  //     newPassword: "Nowe hasło",
  //     currentPassword: "Aktualne hasło",
  //     submit: "Zatwierdź",
  //     changePassword: "Zmień hasło",
      
  //   },
  //   auth: {
  //     errors: {
  //       invalidLoginOrPassword: "Nieprawidłowy login lub hasło.",
  //       missingFields: "Proszę wypełnić wszystkie wymagane pola.",
  //       loginTaken: "Ten login jest już zajęty.",
  //       nicknameTaken: "Ten pseudonim jest już zajęty.",
  //       nicknameTooLong: "Pseudonim jest zbyt długi. (Maksymalnie 15 znaków)"
  //     },
  //     login: "Login",
  //     password: "Hasło",
  //     nickname: "Pseudonim",
  //     register: "Zarejestruj się",
  //     titleLogin: "Logowanie",
  //     titleRegister: "Rejestracja",
  //     questionRegister: "Nie masz konta?",
  //     actionRegister: "Zarejestruj się",
  //     questionLogin: "Masz już konto?",
  //     actionLogin: "Zaloguj się",
  //     email : "Email"
  //   },
  //   toast: {
  //     error: {
  //       generic: "Wystąpił błąd. Spróbuj ponownie.",
  //       avatar: "Nie udało się zaktualizować avatara.",
  //       bio: "Nie udało się zaktualizować opisu.",
  //       login: "Nie udało się zalogować.",
  //       register: "Nie udało się zarejestrować.",
  //       avatarNoFile: "Proszę wybrać plik avatara.",
  //       logout: "Nie udało się wylogować.",
  //       bioTooLong: "Opis jest zbyt długi.",
  //       alreadyInGame: "Już jesteś w grze.",
  //       alreadyInQueue: "Już jesteś w kolejce.",
  //       didntLeaveQueue: "Nie udało się opuścić kolejki.",
  //       unableToJoinGame: "Nie udało się dołączyć do gry.",
  //       pgnCopied: "Nie udało się skopiować PGN do schowka.",
  //       deleteAccount: "Nie udało się usunąć konta.",
  //       nicknameExists: "Pseudonim już istnieje.",
  //       loginExists: "Login już istnieje.",
  //       changePassword: "Nie udało się zmienić hasła.",
  //       wrongPassword: "Błędne aktualne hasło"

  //     },
  //     success: {
  //       generic: "Operacja zakończona pomyślnie.",
  //       avatar: "Avatar zaktualizowany pomyślnie!",
  //       bio: "Opis zaktualizowany pomyślnie!",
  //       login: "Pomyślnie zalogowano!",
  //       register: "Pomyślnie zarejestrowano!",
  //       logout: "Pomyślnie wylogowano!",
  //       leftQueue: "Pomyślnie opuszczono kolejkę!",
  //       pgnCopied: "PGN skopiowany do schowka!",
  //       deleteAccount: "Konto usunięte pomyślnie!",
  //       accountCreated: "Konto utworzone pomyślnie!",
  //       changePassword: "Hasło zmienione pomyślnie!"
  //     },
  //     info: {
  //       addedToQueue: "Zostałeś dodany do kolejki.",
  //       leftQueue: "Opuszczono kolejkę.",
  //       drawOffered: "Remis został zaproponowany.",
  //       isOnline: "Twój znajomy {nickname} jest teraz online.",
  //       isOffline: "Twój znajomy {nickname} jest teraz offline.",
  //       logoutBySystem: "Zostałeś wylogowany"
  //     }
  //   },
  //   gameHistory: {
  //     players: "Gracze",
  //     result: "Wynik",
  //     date: "Data",
  //     noGames: "Brak rozegranych gier...",
  //     analysis : "Analiza",
  //     review: "Przegląd"
  //   },
  //   loading: {
  //     message: "Ładowanie..."
  //   },
  //   search: {
  //     nickname: "Nazwa",
  //     rating: "Ranking",
  //     status: "Status",
  //     last: "Ostatnia",
  //     active: "aktywność",
  //     placeholder: "Szukaj graczy...",
  //     ago: "temu",
  //     hour: "g",
  //     now: "teraz",
  //     page: "Strona",
  //     entriesPerPage: "Wpisów na stronę",
  //     rankingMinMax: "Ranking (Min-Maks)",
  //     onlyActive: "Tylko aktywni",
  //     resetFilters: "Resetuj filtry",
  //     max: "Maks",
  //     min: "Min",
  //     noGamesFound: "Brak gier spełniających kryteria.",
  //     findGame: "Szukaj gry",
  //     all: "Wszystkie",
  //     finished: "Zakończone",
  //     live: "W trakcie",
  //     gamePlaceHolder: "Szukaj po pseudonimie...",
  //     noMoves: "Brak ruchów do wyświetlenia.",
  //     date: "Data",
  //     type: "Typ",
  //     time : "Czas",
  //     moves: "Ruchy",
  //     players: "Gracze",
  //     createUser: "Utwórz użytkownika"

  //   },
  //   home: {
  //     welcome: " Witaj w ChessSite!",
  //     playersOnline: "graczy online",
  //     matchesInProgress: "rozgrywek w toku",
  //     createdAccounts: "utworzonych kont",
  //     wantToPlay: "Chcesz zagrać teraz?",
  //     quickQueue: "Szybka kolejka do gry",
  //     lookingForSomeone: "Chcesz kogoś znaleźć?",
  //     inQueue: "W trakcie szukania gry...",
  //     findMatch: "Znajdź mecz",
  //     find: "Szukaj gry",
  //     queueSize : "w kolejce",
  //     wantToFindGame: "Chcesz znaleźć grę w bazie?",
      
  //     quickMenu: {
  //       queue: "Szybka kolejka",
  //       leaderBoard: "Leaderboard",
  //       friendsOnline: "Znajomi online",
  //       lobby: "Poczekalnia"
  //     },
  //     leaderboard: {
  //       title: "Leaderboard",
  //       player: "Gracz",
  //       rating: "Ranking",
  //       noData: "Brak dostępnych danych"
  //     },
  //     lobby:{
  //       player: "Gracz",
  //       rating: "Ranking",
  //       time: "Czas",
  //       noPlayersInQueue: "Brak graczy w kolejce"
  //     },
      
        
      
  //   },
  //   smallMenu:{
  //     findGame: "Znajdź grę",
  //     findSomeone: "Szukaj innych graczy",
  //     play: "Graj"
  //   },
  //   findGame:{
  //     startGame: "Rozpocznij grę",
  //     cancel: "Anuluj",
  //     rapid: "Szybkie",
  //     blitz: "Blitz",
  //     bullet: "Bullet",
  //     lookingForOpponent: "Szukam przeciwnika..."

  //   },
  //   createAccount:{
  //     required:{
  //       login: "Login jest wymagany.",
  //       nickname: "Nickname jest wymagany.",
  //       password: "Hasło jest wymagane.",
  //       rating : "Ranking jest wymagany."
  //     },
  //     wrongValue:{
  //       nicknameTaken: "Ten nick jest już zajęty.",
  //       loginTaken: "Ten login jest już zajęty.",
  //       bioTooLong: "Opis jest zbyt długi.",
  //       ratingMax: "Ranking nie może przekraczać 3000.",
  //       ratingMin: "Ranking nie może być mniejszy niż 0."
  //     },
  //     addAccount : "Utwórz konto",
  //     bioPlaceholder: "Opis profilu...",
  //     blitzRating: "Ranking Blitz",
  //     rapidRating: "Ranking Rapid",
  //     bulletRating: "Ranking Bullet",
  //     nicknameTaken: "Ten pseudonim jest już zajęty.",
  //     loginTaken: "Ten login jest już zajęty.",
  //     nickname: "Pseudonim",
  //     password: "Hasło",
  //     submit : "Zatwierdź",
  //     editAccount: "Edytuj konto"
      
  //   },
  //   modal:{
  //     cancel: "Anuluj",
  //     confirm: "Potwierdź",
  //     deleteAccount:{
  //       title: "Czy na pewno?",
  //       message: "Tej operacji nie można cofnąć."
  //     }
  //   }

  // },
  
  en: {
    friendAdded: "Friend added successfully!",
    friendAddError: "Failed to add friend.",
    friendDeleted: "Friend deleted successfully!",
    game: {
      chat: {
        title: "Game Chat",
        send: "Send",
        placeholder: "Type a message..."
      },
      title:{
        won:  "Won!",
      lost: "Lost!",
      youWon: "You won!",
      youLost: "You lost!",
      draw: "Draw!",
      },
      reason:{
        threefoldRepetition: "Threefold repetition draw",
        insufficientMaterial: "Insufficient material draw",
        timeout: "Timeout",
        surrender: "Surrender",
        agreement: "Draw by agreement",
        stalemate: "Stalemate draw",
        checkmate: "Win by checkmate",
        disconnect: "Win by opponent disconnect"
      },
      offerDraw: "Offer draw",
      drawOffered: "Draw offered",
      surrender: "Surrender",
      getPGN: "Get PGN",
      acceptDraw: "Accept Draw"

    },
    notFound: {
      title: "Not Found 404",
      backToHome: "Back to home",
      user: "User not found",
      game: "Game not found",
      page: "Page not found"
    },
    navbar: {
      login: "Log in",
      register: "Sign up",
      logout: "Log out",
      inQueue: "In queue",
      leaveQueue: "Leave queue"

    },
    profile: {
      bio: {
        save: "Save",
        cancel: "Cancel",
        edit: "Edit bio"
      },
      avatar: {
        change: "Change avatar"
      },
      userInfo: {
        rating: "Rating",
        joinDate: "Join Date",
        gamesPlayed: "Games",
        wins: "Wins",
        losses: "Losses",
        draws: "Draws"
      },
      notfound: "User not found",
      backToHome: "Back to home",
      addFriend: "Add friend",
      deleteFriend: "Delete friend",
      gameHistory: "Game History",
      friendList: "Friend List",
      noFriends: "No friends",
      friends: "Friends",
      online: "Online",
      offline: "Offline",
      delete: "Account has been deleted.",
      deleteAccount: "Delete Account",
      rating: "Rating",
      eloChart: "Elo Chart",
      newPassword: "New Password",
      currentPassword: "Current Password",
      submit: "Submit",
      changePassword: "Change Password",
      
    },
    auth: {

      login: "Login",
      password: "Password",
      nickname: "Nickname",
      register: "Register",
      titleLogin: "Login",
      titleRegister: "Register",
      questionRegister: "Don't have an account?",
      actionRegister: "Sign up",
      questionLogin: "Already have an account?",
      actionLogin: "Log in",
      email : "Email"
    },
    toast: {
      error: {
        generic: "An error occurred. Please try again.",
        avatar: "Failed to update avatar.",
        bio: "Failed to update bio.",
        login: "Failed to log in.",
        register: "Failed to register.",
        avatarNoFile: "Please select an avatar file.",
        logout: "Failed to log out.",
        bioTooLong: "Bio is too long.",
        alreadyInGame: "You are already in a game.",
        alreadyInQueue: "You are already in the queue.",
        didntLeaveQueue: "Failed to leave the queue.",
        unableToJoinGame: "Unable to join the game.",
        pgnCopied: "Failed to copy PGN to clipboard.",
        deleteAccount: "Failed to delete account.",
        nicknameExists: "Nickname already exists.",
        loginExists: "Login already exists.",
        changePassword: "Failed to change password.",
        wrongPassword: "Wrong current password",

        invalidLoginOrPassword: "Invalid login or password.",
        missingFields: "Please fill in all required fields.",
        loginTaken: "This login is already taken.",
        nicknameTaken: "This nickname is already taken.",
        emailTaken: "This email is already taken.",
        nicknameTooLong: "Nickname is too long. (Max 15 characters)",
        invalidEmail: "Invalid email address.",
        requiredNickname: "Nickname is required.",
        requiredLogin: "Login is required.",
        requiredEmail: "Email is required.",
        requiredPassword: "Password is required.",
        nicknameTooShort: "Nickname is too short. (Min 3 characters)",
        loginTooShort: "Login is too short. (Min 3 characters)",
        passwordTooShort: "Password is too short. (Min 6 characters)"

      },
      success: {
        generic: "Operation completed successfully.",
        avatar: "Avatar updated successfully!",
        bio: "Bio updated successfully!",
        login: "Logged in successfully!",
        register: "Registered successfully!",
        logout: "Logged out successfully!",
        leftQueue: "Left the queue successfully!",
        pgnCopied: "PGN copied to clipboard!",
        deleteAccount: "Account deleted successfully!",
        accountCreated: "Account created successfully!",
        changePassword: "Password changed successfully!"
      },
      info: {
        addedToQueue: "You have been added to the queue.",
        leftQueue: "You have left the queue.",
        drawOffered: "A draw has been offered.",
        isOnline: "Your friend {nickname} is now online.",
        isOffline: "Your friend {nickname} is now offline.",
        logoutBySystem: "You have been logged out"
      }
    },
    gameHistory: {
      players: "Players",
      result: "Result",
      date: "Date",
      noGames: "No games played...",
      analysis : "Analysis",
      review: "Review"
    },
    loading: {
      message: "Loading..."
    },
    search: {
      nickname: "Name",
      rating: "Rating",
      status: "Status",
      last: "Last",
      active: "Active",
      placeholder: "Search for players...",
      ago: "ago",
      hour: "h",
      now: "now",
      page: "Page",
      entriesPerPage: "Entries per page",
      rankingMinMax: "Rating (Min-Max)",
      onlyActive: "Only Active",
      resetFilters: "Reset Filters",
      max: "Max",
      min: "Min",
      noGamesFound: "No games found matching the criteria.",
      findGame: "Find Game",
      all: "All",
      finished: "Finished",
      gamePlaceHolder: "Search by nickname...",
      noMoves: "No moves to display.",
      live: "Live",
      date: "Date",
      type: "Type",
      time : "Time",
      moves: "Moves",
      players: "Players",
      createUser: "Create User"
    },
    home: {
      welcome: " Welcome to ChessSite!",
      playersOnline: "players online",
      matchesInProgress: "matches in progress",
      createdAccounts: "created accounts",
      wantToPlay: "Want to play now?",
      quickQueue: "Quick game queue",
      lookingForSomeone: "Looking for someone?",
      inQueue: "Looking for a game...",
      findMatch: "Find match",
      find: "Find game",
      queueSize : "in queue",
      wantToFindGame: "Want to find a game in the database?",
      quickMenu: {
        queue: "Quick Queue",
        leaderBoard: "Leaderboard",
        friendsOnline: "Friends Online",
        lobby: "Lobby"

      },
      leaderboard: {
        title: "Leaderboard",
        player: "Player",
        rating: "Rating",
        noData: "No data available"
      },
      lobby:{
        player: "Player",
        rating: "Rating",
        time: "Time",
        noPlayersInQueue: "No players in queue"
      },
      
    },
    smallMenu:{
      findGame: "Find game",
      findSomeone: "Find other players",
      play: "Play"
    },
    findGame:{
      startGame: "Start Game",
      cancel: "Cancel",
      rapid: "Rapid",
      blitz: "Blitz",
      bullet: "Bullet",
      lookingForOpponent: "Looking for opponent..."

    },
    createAccount:{
      required:{
        login: "Login is required.",
        nickname: "Nickname is required.",
        password: "Password is required.",
        rating : "Rating is required."
      },
      wrongValue:{
        nicknameTaken: "This nickname is already taken.",
        loginTaken: "This login is already taken.",
        bioTooLong: "Bio is too long.",
        ratingMax: "Rating cannot exceed 3000.",
        ratingMin: "Rating cannot be less than 0."
      },
      addAccount : "Create Account",
      bioPlaceholder: "Profile bio...",
      blitzRating: "Blitz Rating",
      rapidRating: "Rapid Rating",
      bulletRating: "Bullet Rating",
      nicknameTaken: "This nickname is already taken.",
      loginTaken: "This login is already taken.",
      nickname: "Nickname",
      password: "Password",
      submit : "Submit",
      editAccount: "Edit Account"
    },
    modal:{
      cancel: "Cancel",
      confirm: "Confirm",
      deleteAccount:{
        title: "Are you sure?",
        message: "This action cannot be undone."
      },
      
    }
  
  }
};

export type Language = keyof typeof translations