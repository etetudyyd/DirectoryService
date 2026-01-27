"use client"

import Link from "next/link";
import { Search, Bell, Settings, User } from "lucide-react";
import routes from "@/shared/routes";
import { SidebarTrigger } from "@/shared/components/ui/sidebar";

export default function Header() {
  return (
    <header className="w-full h-16 bg-gray-900/90 backdrop-blur-sm border-b border-gray-800/50 text-white flex items-center px-4 sm:px-6 lg:px-8">
      <div className="flex items-center gap-3">
        <SidebarTrigger className="md:hidden h-9 w-9 hover:bg-gray-800/50 transition-colors" />
        <div className="hidden md:flex h-10 w-10 items-center justify-center rounded-lg bg-gradient-to-br from-indigo-600 to-sky-500 text-white font-bold shadow-lg">
          DS
        </div>
        <Link href={routes.home} className="group">
          <h1 className="text-lg font-semibold text-white group-hover:text-blue-300 transition-colors">
            Directory Service
          </h1>
          <div className="text-xs text-gray-400 group-hover:text-gray-300 transition-colors">
            Management web-service
          </div>
        </Link>
      </div>

      <div className="flex-1" />
      
      <div className="flex items-center gap-2">
        {/* Search - скрыто на мобильных */}
        <div className="hidden md:flex items-center bg-gray-800/50 border border-gray-700 rounded-lg px-3 py-2">
          <Search className="h-4 w-4 text-gray-400 mr-2" />
          <input
            type="text"
            placeholder="Search..."
            className="bg-transparent text-sm text-white placeholder-gray-400 outline-none w-40"
          />
        </div>

        <div className="flex items-center gap-1">
          <button 
            aria-label="Notifications" 
            className="p-2 rounded-lg hover:bg-gray-800/70 transition-colors relative group"
          >
            <Bell className="h-5 w-5 text-gray-300 group-hover:text-white" />
            <span className="absolute -top-1 -right-1 h-2 w-2 bg-red-500 rounded-full animate-pulse" />
          </button>

          <Link 
            href="/settings" 
            className="p-2 rounded-lg hover:bg-gray-800/70 transition-colors group"
          >
            <Settings className="h-5 w-5 text-gray-300 group-hover:text-white" />
          </Link>

          {/* User profile - скрыто на мобильных */}
          <div className="hidden md:flex items-center gap-2 pl-2 border-l border-gray-700/50">
            <div className="h-8 w-8 rounded-full bg-linear-to-br from-gray-700 to-gray-900 border border-gray-600 flex items-center justify-center">
              <User className="h-4 w-4 text-gray-300" />
            </div>
            <div className="text-sm">
              <div className="font-medium text-white">Admin User</div>
              <div className="text-xs text-gray-400">Administrator</div>
            </div>
          </div>
        </div>
      </div>
    </header>
  );
}