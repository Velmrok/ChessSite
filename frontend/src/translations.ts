
export const translations = {
  en: {
    friendAdded: "Friend added successfully!",
    friendAddError: "Failed to add friend.",
    friendDeleted: "Friend deleted successfully!",
    game: {
      

    },

  

  
   
    loading: {
      message: "Loading..."
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