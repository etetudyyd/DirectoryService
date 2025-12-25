"use client"

import Link from "next/link";
import { Home, Info, Mail, MapPin, User, Warehouse } from "lucide-react";
import {
    Sidebar,
    SidebarContent,
    SidebarGroup,
    SidebarGroupContent,
    SidebarHeader,
    SidebarMenu,
    SidebarMenuItem,
    SidebarMenuButton,
    SidebarTrigger,
    SidebarFooter,
    SidebarSeparator,
} from "../ui/sidebar";
import routes from "@/shared/routes";
import { usePathname } from "next/navigation";

const items = [
    { label: "Departments", icon: Warehouse, href: routes.departments},
    { label: "Positions", icon: User, href: routes.positions },
    { label: "Locations", icon: MapPin, href: routes.locations },
];

export function AppSidebar() {
    const pathname = usePathname();

    return (
            <Sidebar collapsible="icon">
                <SidebarHeader className="py-4">
                        <SidebarTrigger />
                </SidebarHeader>

                <SidebarContent className="py-4">           
                    <SidebarGroup>
                        <SidebarGroupContent>
                            <SidebarMenu className="mb-2">
                                {items.map((it) => (
                                    <SidebarMenuItem key={it.href}>
                                        <SidebarMenuButton asChild isActive={pathname === it.href}>
                                            <Link href={it.href} className="flex w-full items-center gap-2">
                                                <it.icon />
                                                <span>{it.label}</span>
                                            </Link>
                                        </SidebarMenuButton>
                                    </SidebarMenuItem>
                                ))}
                            </SidebarMenu>
                        </SidebarGroupContent>
                    </SidebarGroup>
                </SidebarContent>
            </Sidebar>
    );
}