import { Routes, Route } from 'react-router-dom'
import Home from './pages/Home'
import NotFound from './pages/NotFound'
import Navbar from './components/global/Navbar'
import UserProfilePage from './pages/UserProfilePage'
import Loading from './components/global/Loading'
import GamePage from './pages/GamePage'
import ToastContainer from './components/global/ToastContainer'
import FindGame from './pages/FindGame'
import {OnlyLoggedInRoute} from './components/global/ProtectedRoute'
import RegisterPage from './pages/RegisterPage'
import LoginPage from './pages/LoginPage'
import { useAuth } from './hooks/useAuth'
import UsersSearchPage from './pages/UsersSearchPage'
import GamesSearchPage from './pages/GamesSearchPage'
import { useEffect } from 'react'
import { connectSignalR } from './services/signalR/connection'
import { useGlobalSignalR } from './hooks/useGlobalSocket'

function App() {
  
  
  const { loading } = useAuth();
  
  useEffect(() => {
    connectSignalR();
    
  }, []);
  useGlobalSignalR();

  

  return (
    <>
   

    <Navbar/>
    {loading ? <>
    <div className="bg-cyan-800 w-full min-h-screen flex justify-center items-center"> 
    <Loading /> 
    </div>
    </> :
      <Routes>
        <Route path="/" element={<Home />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            
           
            <Route element={<OnlyLoggedInRoute />}>
              <Route path="/find-game" element={<FindGame />} />
              <Route path="/search" element={<UsersSearchPage />} />
               <Route path="/game/:gameId" element={<GamePage />} />
               <Route path ="/games" element={<GamesSearchPage/>} />
               <Route path="/users/:nickname/profile" element={<UserProfilePage />} />
            
            </Route>


            <Route path="*" element={<NotFound whatIsMissing='page'/>} />
            
      </Routes>}
     

    <ToastContainer/>
    </>
  )
}

export default App
