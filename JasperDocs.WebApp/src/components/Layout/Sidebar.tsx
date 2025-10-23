import { NavLink } from 'react-router-dom';
import { Stack } from '@mantine/core';
import { IconFiles } from '@tabler/icons-react';
import classes from './Sidebar.module.css';

export function Sidebar() {
  return (
    <Stack gap="xs" p="md">
      <NavLink
        to="/documents"
        className={({ isActive }) =>
          isActive ? `${classes.link} ${classes.active}` : classes.link
        }
      >
        <IconFiles size={20} stroke={1.5} />
        <span>Documents</span>
      </NavLink>
    </Stack>
  );
}
