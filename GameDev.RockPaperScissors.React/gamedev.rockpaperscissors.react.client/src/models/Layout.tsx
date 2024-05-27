import { ReactNode } from "react";
import TopNavbar from "../components/TopNavbar";

interface Props {
	children: ReactNode
}

export default function Layout({ children }: Props) {
	return (
		<div>
			<TopNavbar />
			{children}
		</div>
	);
}