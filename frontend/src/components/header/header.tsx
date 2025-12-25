"use client"

import Link from "next/link";
import { Search, Bell, Settings, User } from "lucide-react";
import routes from "@/shared/routes";
import { SidebarTrigger } from "../ui/sidebar";

export default function Header() {
  return (
    <header className="w-full h-16 bg-transparent text-white flex items-center px-4">
        
      <div className="flex items-center gap-4">
        <SidebarTrigger className="md:hidden" />
        <div className="flex h-10 w-10 items-center justify-center rounded-md bg-linear-to-br from-indigo-600 to-sky-500 text-white font-bold">
          DS
        </div>
        <Link href={routes.home}>
          <h1 className="text-lg font-semibold">Directory Service</h1>
          <div className="text-xs text-gray-300">Management web-service</div>
        </Link>
      </div>

      <div className="flex-1" />

      <div className="flex items-center gap-3">
        <div className="hidden md:flex items-center bg-gray-700 rounded-md px-2 py-1 gap-2">
          <Search className="text-gray-300" />
          <input
            placeholder="Search..."
            className="bg-transparent outline-none text-sm text-white placeholder-gray-400"
          />
        </div>

        <button aria-label="Notifications" className="p-2 rounded-md hover:bg-gray-700">
          <Bell />
        </button>

        <Link href="/settings" className="p-2 rounded-md hover:bg-gray-700">
          <Settings />
        </Link>

      </div>
    </header>
  );
}